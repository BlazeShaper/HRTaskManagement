using System;
using HRTaskManagement.Domain.Common;

namespace HRTaskManagement.Domain.Entities
{
    public class AssetAssignment : BaseEntity
    {
        public Guid AssetId { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid AssignedByUserId { get; set; }     // Atamayı yapan kullanıcı (İK/Admin)
        public DateTime AssignedDate { get; set; } = DateTime.UtcNow;
        public DateTime? ReturnedDate { get; set; }
        public string? Note { get; set; }

        // Navigation Properties
        public Asset? Asset { get; set; }
        public Employee? Employee { get; set; }
        public User? AssignedByUser { get; set; }
    }
}