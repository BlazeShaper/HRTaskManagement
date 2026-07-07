namespace HRTaskManagement.Application.DTOs.LeaveRequest
{
    public class LeaveRequestDto
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public string EmployeeFullName { get; set; } = string.Empty;
        public string LeaveType { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalDays => (EndDate - StartDate).Days + 1;
        public string Status { get; set; } = string.Empty;
        public string? Reason { get; set; }
        public Guid? ApprovedBy { get; set; }
        public string? ApprovedByUsername { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
