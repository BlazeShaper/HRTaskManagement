// Persistence/Services/LogService.cs
using Microsoft.EntityFrameworkCore;
using HRTaskManagement.Application.DTOs.Common;
using HRTaskManagement.Application.DTOs.Log;
using HRTaskManagement.Application.Interfaces;
using HRTaskManagement.Domain.Entities;
using HRTaskManagement.Persistence.Context;

namespace HRTaskManagement.Persistence.Services
{
    public class LogService : ILogService
    {
        private readonly WorkSphereDbContext _dbContext;

        public LogService(WorkSphereDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PagedResult<LogDto>> GetAllAsync(LogQueryParameters queryParameters)
        {
            var query = _dbContext.Logs.AsNoTracking().AsQueryable();

            if (queryParameters.UserId.HasValue)
                query = query.Where(l => l.UserId == queryParameters.UserId.Value);

            if (!string.IsNullOrWhiteSpace(queryParameters.Action))
                query = query.Where(l => l.Action.ToLower().Contains(
                    queryParameters.Action.Trim().ToLower()));

            if (!string.IsNullOrWhiteSpace(queryParameters.EntityName))
                query = query.Where(l => l.EntityName.ToLower() ==
                    queryParameters.EntityName.Trim().ToLower());

            if (queryParameters.EntityId.HasValue)
                query = query.Where(l => l.EntityId == queryParameters.EntityId.Value);

            if (queryParameters.FromDate.HasValue)
                query = query.Where(l => l.CreatedDate >= queryParameters.FromDate.Value);

            if (queryParameters.ToDate.HasValue)
                query = query.Where(l => l.CreatedDate <= queryParameters.ToDate.Value);

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(l => l.CreatedDate)
                .Skip((queryParameters.PageNumber - 1) * queryParameters.PageSize)
                .Take(queryParameters.PageSize)
                .Select(l => new LogDto
                {
                    Id = l.Id,
                    UserId = l.UserId,
                    Username = l.User != null ? l.User.Username : null,
                    Action = l.Action,
                    EntityName = l.EntityName,
                    EntityId = l.EntityId,
                    IpAddress = l.IpAddress,
                    CreatedDate = l.CreatedDate
                })
                .ToListAsync();

            return new PagedResult<LogDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = queryParameters.PageNumber,
                PageSize = queryParameters.PageSize
            };
        }

        public async Task<LogDto> GetByIdAsync(Guid id)
        {
            var log = await _dbContext.Logs
                .AsNoTracking()
                .Where(l => l.Id == id)
                .Select(l => new LogDto
                {
                    Id = l.Id,
                    UserId = l.UserId,
                    Username = l.User != null ? l.User.Username : null,
                    Action = l.Action,
                    EntityName = l.EntityName,
                    EntityId = l.EntityId,
                    IpAddress = l.IpAddress,
                    CreatedDate = l.CreatedDate
                })
                .FirstOrDefaultAsync();

            return log ?? throw new KeyNotFoundException("Log kaydı bulunamadı.");
        }

        public async Task LogActionAsync(
            Guid? userId, string action, string entityName, Guid entityId, string? ipAddress)
        {
            var logEntry = new Log
            {
                UserId = userId,
                Action = action,
                EntityName = entityName,
                EntityId = entityId,
                IpAddress = ipAddress
            };

            _dbContext.Logs.Add(logEntry);
            await _dbContext.SaveChangesAsync();
        }
    }
}
