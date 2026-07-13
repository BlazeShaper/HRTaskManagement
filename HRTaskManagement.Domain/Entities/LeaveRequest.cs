// Entities/LeaveRequest.cs
using HRTaskManagement.Domain.Common;
using HRTaskManagement.Domain.Enums;

namespace HRTaskManagement.Domain.Entities
{
    public class LeaveRequest : BaseEntity
    {
        public Guid EmployeeId { get; set; }
        public LeaveType LeaveType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public LeaveStatus Status { get; set; } = LeaveStatus.Pending;
        public string? Reason { get; set; }
        public Guid? ApprovedBy { get; set; }

        // Navigation Properties
        public Employee? Employee { get; set; }
        public User? ApprovedByUser { get; set; }
    }
}