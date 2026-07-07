namespace HRTaskManagement.Application.DTOs.TaskComment
{
    public class TaskCommentQueryParameters
    {
        private const int MaxPageSize = 50;
        private int _pageSize = 10;

        public Guid? TaskId { get; set; }
        public Guid? EmployeeId { get; set; }
        public int PageNumber { get; set; } = 1;

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
        }
    }
}
