// Persistence/Services/TaskCommentService.cs
using Microsoft.EntityFrameworkCore;
using HRTaskManagement.Application.DTOs.Common;
using HRTaskManagement.Application.DTOs.TaskComment;
using HRTaskManagement.Application.Interfaces;
using HRTaskManagement.Persistence.Context;

namespace HRTaskManagement.Persistence.Services
{
    public class TaskCommentService : ITaskCommentService
    {
        private readonly WorkSphereDbContext _dbContext;

        public TaskCommentService(WorkSphereDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        private static IQueryable<TaskCommentDto> ProjectToDto(
            IQueryable<Domain.Entities.TaskComment> source)
        {
            return source.Select(tc => new TaskCommentDto
            {
                Id = tc.Id,
                TaskId = tc.TaskId,
                EmployeeId = tc.EmployeeId,
                EmployeeFullName = tc.Employee!.FirstName + " " + tc.Employee.LastName,
                Comment = tc.Comment,
                CreatedDate = tc.CreatedDate
            });
        }

        public async Task<PagedResult<TaskCommentDto>> GetAllAsync(
            TaskCommentQueryParameters queryParameters)
        {
            var query = _dbContext.TaskComments
                .AsNoTracking()
                .Where(tc => !tc.IsDeleted);

            if (queryParameters.TaskId.HasValue)
                query = query.Where(tc => tc.TaskId == queryParameters.TaskId.Value);

            if (queryParameters.EmployeeId.HasValue)
                query = query.Where(tc => tc.EmployeeId == queryParameters.EmployeeId.Value);

            var totalCount = await query.CountAsync();

            var items = await ProjectToDto(query)
                .OrderByDescending(tc => tc.CreatedDate)
                .Skip((queryParameters.PageNumber - 1) * queryParameters.PageSize)
                .Take(queryParameters.PageSize)
                .ToListAsync();

            return new PagedResult<TaskCommentDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = queryParameters.PageNumber,
                PageSize = queryParameters.PageSize
            };
        }

        public async Task<TaskCommentDto> GetByIdAsync(Guid id)
        {
            var query = _dbContext.TaskComments
                .AsNoTracking()
                .Where(tc => tc.Id == id && !tc.IsDeleted);

            var result = await ProjectToDto(query).FirstOrDefaultAsync();

            return result ?? throw new KeyNotFoundException("Yorum bulunamadı.");
        }

        public async Task<TaskCommentDto> CreateAsync(CreateTaskCommentDto dto, Guid currentUserId)
        {
            // Görev var mı kontrolü
            var taskExists = await _dbContext.Tasks
                .AnyAsync(t => t.Id == dto.TaskId && !t.IsDeleted);

            if (!taskExists)
                throw new KeyNotFoundException("Görev bulunamadı.");

            // Giriş yapan kullanıcının çalışan kaydını bul (UserId üzerinden)
            var employee = await _dbContext.Employees
                .FirstOrDefaultAsync(e => e.UserId == currentUserId && !e.IsDeleted)
                ?? throw new InvalidOperationException(
                    "Bu kullanıcıya bağlı bir çalışan kaydı bulunamadı.");

            var comment = new Domain.Entities.TaskComment
            {
                TaskId = dto.TaskId,
                EmployeeId = employee.Id,
                Comment = dto.Comment.Trim()
            };

            _dbContext.TaskComments.Add(comment);
            await _dbContext.SaveChangesAsync();

            return await GetByIdAsync(comment.Id);
        }

        public async Task DeleteAsync(Guid id, Guid currentUserId, bool isManagerOrAbove)
        {
            var comment = await _dbContext.TaskComments
                .FirstOrDefaultAsync(tc => tc.Id == id && !tc.IsDeleted)
                ?? throw new KeyNotFoundException("Yorum bulunamadı.");

            // Manager değilse yalnızca kendi yorumunu silebilir
            if (!isManagerOrAbove)
            {
                var employee = await _dbContext.Employees
                    .FirstOrDefaultAsync(e => e.UserId == currentUserId && !e.IsDeleted);

                if (employee is null || comment.EmployeeId != employee.Id)
                    throw new UnauthorizedAccessException("Sadece kendi yorumunuzu silebilirsiniz.");
            }

            comment.IsDeleted = true;
            comment.UpdatedDate = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
        }
    }
}
