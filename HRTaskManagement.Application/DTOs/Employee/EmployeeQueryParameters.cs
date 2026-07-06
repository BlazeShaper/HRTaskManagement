// Application/DTOs/Employee/EmployeeQueryParameters.cs
namespace HRTaskManagement.Application.DTOs.Employee
{
    public class EmployeeQueryParameters
    {
        // Arama — Ad, Soyad veya Email içinde geçen metin
        public string? SearchTerm { get; set; }

        // Filtreleme
        public Guid? DepartmentId { get; set; }
        public Guid? PositionId { get; set; }
        public bool? IsActive { get; set; }

        // Sayfalama
        private const int MaxPageSize = 50;
        private int _pageSize = 10;

        public int PageNumber { get; set; } = 1;

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
        }
    }
}