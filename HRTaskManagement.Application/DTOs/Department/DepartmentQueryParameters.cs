using System;

namespace HRTaskManagement.Application.DTOs.Department
{
    public class DepartmentQueryParameters
    {
        // Arama — Departman adı veya açıklaması
        public string? SearchTerm { get; set; }

        // Filtre — Yönetici Id'sine göre filtreleme
        public Guid? ManagerId { get; set; }

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
