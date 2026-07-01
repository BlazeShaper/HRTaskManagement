namespace HRTaskManagement.Domain.Common
{
    public abstract class BaseEntity
    {
        // Employee.cs içine ekleyin (henüz eklemediyseniz):
        public ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}