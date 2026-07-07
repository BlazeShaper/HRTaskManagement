namespace HRTaskManagement.Application.DTOs.Notification
{
    public class NotificationQueryParameters
    {
        private const int MaxPageSize = 50;
        private int _pageSize = 10;

        public bool? IsRead { get; set; }
        public string? Type { get; set; }
        public int PageNumber { get; set; } = 1;

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
        }
    }
}
