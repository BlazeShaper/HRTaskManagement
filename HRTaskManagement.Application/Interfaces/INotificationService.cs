using HRTaskManagement.Application.DTOs.Common;
using HRTaskManagement.Application.DTOs.Notification;

namespace HRTaskManagement.Application.Interfaces
{
    public interface INotificationService
    {
        Task<PagedResult<NotificationDto>> GetMyNotificationsAsync(
            Guid userId, NotificationQueryParameters queryParameters);
        Task<NotificationDto> GetByIdAsync(Guid id, Guid userId);
        Task<NotificationDto> CreateAsync(CreateNotificationDto dto);
        Task MarkAsReadAsync(Guid id, Guid userId);
        Task MarkAllAsReadAsync(Guid userId);
        Task<int> GetUnreadCountAsync(Guid userId);
    }
}
