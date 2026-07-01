// Entities/Notification.cs
using System;
using HRTaskManagement.Domain.Common;
using HRTaskManagement.Domain.Enums;

namespace HRTaskManagement.Domain.Entities
{
    public class Notification : BaseEntity
    {
        public Guid UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; } = false;
        public NotificationType Type { get; set; } = NotificationType.General;

        // Navigation Property
        public User? User { get; set; }
    }
}