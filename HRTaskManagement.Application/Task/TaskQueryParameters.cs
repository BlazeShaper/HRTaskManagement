using System;

namespace HRTaskManagement.Application.DTOs.Task
{
    public class TaskQueryParameters
    {
        // Arama — Görev başlığı veya açıklaması
        public string? SearchTerm { get; set; }

        // Filtre — Görev durumu (Pending, InProgress, Done, Cancelled)
        public string? Status { get; set; }

        // Filtre — Görevin atandığı çalışan
        public Guid? EmployeeId { get; set; }

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
