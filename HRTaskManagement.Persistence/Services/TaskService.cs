// Persistence/Services/TaskService.cs
using Microsoft.EntityFrameworkCore;
using HRTaskManagement.Application.DTOs.Common;
using HRTaskManagement.Application.DTOs.Task;
using HRTaskManagement.Application.DTOs.Notification;
using HRTaskManagement.Application.Interfaces;
using HRTaskManagement.Persistence.Context;
using HRTaskManagement.Shared.Constants;
using TaskItemEntity = HRTaskManagement.Domain.Entities.TaskItem;

namespace HRTaskManagement.Persistence.Services
{
    public class TaskService : ITaskService
    {
        private readonly WorkSphereDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly INotificationService _notificationService;

        public TaskService(
            WorkSphereDbContext context,
            ICurrentUserService currentUserService,
            INotificationService notificationService)
        {
            _context = context;
            _currentUserService = currentUserService;
            _notificationService = notificationService;
        }

        public async Task<PagedResult<TaskDto>> GetAllAsync(TaskQueryParameters queryParameters)
        {
            IQueryable<TaskItemEntity> query = _context.Tasks
                .AsNoTracking();

            // Arama — Başlık veya açıklama içinde ara
            if (!string.IsNullOrWhiteSpace(queryParameters.SearchTerm))
            {
                var search = queryParameters.SearchTerm.Trim().ToLower();
                query = query.Where(t => t.Title.ToLower().Contains(search) ||
                                         (t.Description != null && t.Description.ToLower().Contains(search)));
            }

            // Filtre — Duruma göre filtrele
            if (!string.IsNullOrWhiteSpace(queryParameters.Status) &&
                Enum.TryParse<Domain.Enums.TaskStatus>(queryParameters.Status, true, out var statusFilter))
            {
                query = query.Where(t => t.Status == statusFilter);
            }

            // Filtre — Atanan çalışana göre filtrele
            if (queryParameters.EmployeeId.HasValue)
            {
                query = query.Where(t => t.EmployeeId == queryParameters.EmployeeId.Value);
            }

            int totalCount = await query.CountAsync();

            var tasks = await query
                .OrderByDescending(t => t.CreatedDate)
                .Skip((queryParameters.PageNumber - 1) * queryParameters.PageSize)
                .Take(queryParameters.PageSize)
                .Select(t => new TaskDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    Status = t.Status.ToString(),
                    DueDate = t.DueDate,
                    EmployeeId = t.EmployeeId,
                    EmployeeFullName = t.Employee != null
                        ? t.Employee.FirstName + " " + t.Employee.LastName
                        : string.Empty,
                    CommentCount = t.Comments.Count()
                })
                .ToListAsync();

            return new PagedResult<TaskDto>
            {
                Items = tasks,
                PageNumber = queryParameters.PageNumber,
                PageSize = queryParameters.PageSize,
                TotalCount = totalCount
            };
        }

        public async Task<TaskDto> GetByIdAsync(Guid id)
        {
            var task = await _context.Tasks
                .AsNoTracking()
                .Where(t => t.Id == id)
                .Select(t => new TaskDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    Status = t.Status.ToString(),
                    DueDate = t.DueDate,
                    EmployeeId = t.EmployeeId,
                    EmployeeFullName = t.Employee != null
                        ? t.Employee.FirstName + " " + t.Employee.LastName
                        : string.Empty,
                    CommentCount = t.Comments.Count()
                })
                .FirstOrDefaultAsync();

            if (task == null)
                throw new KeyNotFoundException($"{id} ID'li görev bulunamadı.");

            return task;
        }

        public async Task<TaskDto> CreateAsync(CreateTaskDto request)
        {
            // KURAL 1: Görevi oluşturan kişi Admin veya Manager olmalı
            bool isAuthorizedCreator = _currentUserService.IsInRole(SystemRoles.Admin)
                || _currentUserService.IsInRole(SystemRoles.Manager);

            if (!isAuthorizedCreator)
                throw new UnauthorizedAccessException(
                    "Görev oluşturma yetkisi sadece Admin veya Manager rolündeki kullanıcılara aittir.");

            // KURAL 2: Görev atanan kişi aktif bir çalışan olmalı
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.Id == request.EmployeeId && e.IsActive);

            if (employee == null)
                throw new InvalidOperationException(
                    "Belirtilen EmployeeId'ye sahip aktif bir çalışan bulunamadı.");

            var task = new TaskItemEntity
            {
                Title = request.Title,
                Description = request.Description,
                EmployeeId = request.EmployeeId,
                DueDate = request.DueDate,
                Status = Domain.Enums.TaskStatus.Pending
            };

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            var assignedEmployee = await _context.Employees
                .FirstOrDefaultAsync(e => e.Id == task.EmployeeId && !e.IsDeleted);

            if (assignedEmployee?.UserId is Guid assignedUserId)
            {
                await _notificationService.CreateAsync(new CreateNotificationDto
                {
                    UserId = assignedUserId,
                    Title = "Yeni Görev Atandı",
                    Message = $"\"{task.Title}\" başlıklı yeni bir görev size atandı.",
                    Type = "TaskAssigned"
                });
            }

            return await GetByIdAsync(task.Id);
        }

        public async Task UpdateAsync(Guid id, UpdateTaskDto request)
        {
            var task = await _context.Tasks.FindAsync(id);

            if (task == null)
                throw new KeyNotFoundException($"{id} ID'li görev bulunamadı.");

            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.Id == request.EmployeeId && e.IsActive);

            if (employee == null)
                throw new InvalidOperationException(
                    "Belirtilen EmployeeId'ye sahip aktif bir çalışan bulunamadı.");

            if (!Enum.TryParse<Domain.Enums.TaskStatus>(request.Status, out var parsedStatus))
                throw new InvalidOperationException(
                    $"'{request.Status}' geçerli bir görev durumu değil.");

            task.Title = request.Title;
            task.Description = request.Description;
            task.EmployeeId = request.EmployeeId;
            task.DueDate = request.DueDate;
            task.Status = parsedStatus;

            await _context.SaveChangesAsync();
        }

        // ============================================
        // YENİ: Sadece durum güncelleme — atanan kişi kontrolü
        // ============================================
        public async Task UpdateStatusAsync(Guid id, UpdateTaskStatusDto request)
        {
            var task = await _context.Tasks.FindAsync(id);

            if (task == null)
                throw new KeyNotFoundException($"{id} ID'li görev bulunamadı.");

            if (!Enum.TryParse<Domain.Enums.TaskStatus>(request.Status, out var parsedStatus))
                throw new InvalidOperationException(
                    $"'{request.Status}' geçerli bir görev durumu değil.");

            // KURAL: Sadece görevin atandığı kişi durumu güncelleyebilir
            var currentUserId = _currentUserService.UserId;

            if (currentUserId == null)
                throw new UnauthorizedAccessException("Kimlik doğrulanamadı.");

            var currentEmployee = await _context.Employees
                .FirstOrDefaultAsync(e => e.UserId == currentUserId.Value);

            if (currentEmployee == null || currentEmployee.Id != task.EmployeeId)
                throw new UnauthorizedAccessException(
                    "Sadece görevin atandığı kişi durumu güncelleyebilir.");

            task.Status = parsedStatus;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var task = await _context.Tasks.FindAsync(id);

            if (task == null)
                throw new KeyNotFoundException($"{id} ID'li görev bulunamadı.");

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
        }
    }
}