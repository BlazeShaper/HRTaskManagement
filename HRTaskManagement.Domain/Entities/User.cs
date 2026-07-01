using System;
using System.Collections.Generic;
using HRTaskManagement.Domain.Common;

namespace HRTaskManagement.Domain.Entities
{
    public class User : BaseEntity
    {
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;

        public Employee? Employee { get; set; }
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
        public ICollection<Log> Logs { get; set; } = new List<Log>();
    }
}