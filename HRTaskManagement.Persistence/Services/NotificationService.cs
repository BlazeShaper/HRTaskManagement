// Persistence/Services/NotificationService.cs
using Microsoft.EntityFrameworkCore;
using HRTaskManagement.Application.DTOs.Common;
using HRTaskManagement.Application.DTOs.Notification;
using HRTaskManagement.Application.Interfaces;
using HRTaskManagement.Domain.Entities;
using HRTaskManagement.Domain.Enums;
using HRTaskManagement.Persistence.Context;

namespace HRTaskManagement.Persistence.Services
{
    public class NotificationService : INotificationService
    {
        private readonly WorkSphereDbContext _dbContext;

        public NotificationService(WorkSphereDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PagedResult<NotificationDto>> GetMyNotificationsAsync(
            Guid userId, NotificationQueryParameters queryParameters)
        {
            var query = _dbContext.Notifications
                .AsNoTracking()
                .Where(n => n.UserId == userId && !n.IsDeleted);

            if (queryParameters.IsRead.HasValue)
                query = query.Where(n => n.IsRead == queryParameters.IsRead.Value);

            if (!string.IsNullOrWhiteSpace(queryParameters.Type) &&
                Enum.TryParse<NotificationType>(queryParameters.Type, true, out var typeFilter))
                query = query.Where(n => n.Type == typeFilter);

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(n => n.CreatedDate)
                .Skip((queryParameters.PageNumber - 1) * queryParameters.PageSize)
                .Take(queryParameters.PageSize)
                .Select(n => new NotificationDto
                {
                    Id = n.Id,
                    UserId = n.UserId,
                    Title = n.Title,
                    Message = n.Message,
                    IsRead = n.IsRead,
                    Type = n.Type.ToString(),
                    CreatedDate = n.CreatedDate
                })
                .ToListAsync();

            return new PagedResult<NotificationDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = queryParameters.PageNumber,
                PageSize = queryParameters.PageSize
            };
        }

        public async Task<NotificationDto> GetByIdAsync(Guid id, Guid userId)
        {
            var notification = await _dbContext.Notifications
                .AsNoTracking()
                .Where(n => n.Id == id && n.UserId == userId && !n.IsDeleted)
                .Select(n => new NotificationDto
                {
                    Id = n.Id,
                    UserId = n.UserId,
                    Title = n.Title,
                    Message = n.Message,
                    IsRead = n.IsRead,
                    Type = n.Type.ToString(),
                    CreatedDate = n.CreatedDate
                })
                .FirstOrDefaultAsync();

            return notification ?? throw new KeyNotFoundException("Bildirim bulunamadı.");
        }

        public async Task<NotificationDto> CreateAsync(CreateNotificationDto dto)
        {
            if (!Enum.TryParse<NotificationType>(dto.Type, true, out var notifType))
                notifType = NotificationType.General;

            var notification = new Notification
            {
                UserId = dto.UserId,
                Title = dto.Title.Trim(),
                Message = dto.Message.Trim(),
                Type = notifType,
                IsRead = false
            };

            _dbContext.Notifications.Add(notification);
            await _dbContext.SaveChangesAsync();

            return new NotificationDto
            {
                Id = notification.Id,
                UserId = notification.UserId,
                Title = notification.Title,
                Message = notification.Message,
                IsRead = notification.IsRead,
                Type = notification.Type.ToString(),
                CreatedDate = notification.CreatedDate
            };
        }

        public async Task MarkAsReadAsync(Guid id, Guid userId)
        {
            var notification = await _dbContext.Notifications
                .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId && !n.IsDeleted)
                ?? throw new KeyNotFoundException("Bildirim bulunamadı.");

            notification.IsRead = true;
            notification.UpdatedDate = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
        }

        public async Task MarkAllAsReadAsync(Guid userId)
        {
            var unread = await _dbContext.Notifications
                .Where(n => n.UserId == userId && !n.IsRead && !n.IsDeleted)
                .ToListAsync();

            foreach (var n in unread)
            {
                n.IsRead = true;
                n.UpdatedDate = DateTime.UtcNow;
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task<int> GetUnreadCountAsync(Guid userId)
        {
            return await _dbContext.Notifications
                .CountAsync(n => n.UserId == userId && !n.IsRead && !n.IsDeleted);
        }
    }
}
