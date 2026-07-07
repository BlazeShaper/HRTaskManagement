namespace HRTaskManagement.Application.DTOs.LeaveRequest
{
    public class LeaveRequestQueryParameters
    {
        private const int MaxPageSize = 50;
        private int _pageSize = 10;

        public Guid? EmployeeId { get; set; }
        public string? Status { get; set; }      // "Pending", "Approved", "Rejected", "Cancelled"
        public string? LeaveType { get; set; }
        public int PageNumber { get; set; } = 1;

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
        }
    }
}
