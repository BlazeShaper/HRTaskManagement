using System.Collections.Generic;
using HRTaskManagement.Domain.Common;

namespace HRTaskManagement.Domain.Entities
{
    public class Role : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
