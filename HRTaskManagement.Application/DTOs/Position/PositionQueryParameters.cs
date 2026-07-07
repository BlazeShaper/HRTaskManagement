// Application/DTOs/Position/PositionQueryParameters.cs
namespace HRTaskManagement.Application.DTOs.Position
{
    public class PositionQueryParameters
    {
        // Arama — Pozisyon başlığı içinde geçen metin
        public string? SearchTerm { get; set; }

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
