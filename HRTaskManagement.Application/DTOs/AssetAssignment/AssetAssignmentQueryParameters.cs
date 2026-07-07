namespace HRTaskManagement.Application.DTOs.AssetAssignment
{
    public class AssetAssignmentQueryParameters
    {
        private const int MaxPageSize = 50;
        private int _pageSize = 10;

        public Guid? EmployeeId { get; set; }
        public Guid? AssetId { get; set; }
        public bool? OnlyActive { get; set; } // true: sadece hâlâ iade edilmemiş zimmetler
        public int PageNumber { get; set; } = 1;

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
        }
    }
}
