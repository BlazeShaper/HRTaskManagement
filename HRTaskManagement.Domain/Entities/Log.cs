// Entities/Log.cs
using System;
using HRTaskManagement.Domain.Common;

namespace HRTaskManagement.Domain.Entities
{
    public class Log : BaseEntity
    {
        public Guid? UserId { get; set; }
        public string Action { get; set; } = string.Empty;
        public string EntityName { get; set; } = string.Empty;
        public Guid EntityId { get; set; }
        public string? IpAddress { get; set; }

        // Navigation Property
        public User? User { get; set; }
    }
}