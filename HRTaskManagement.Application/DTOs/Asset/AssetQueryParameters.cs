namespace HRTaskManagement.Application.DTOs.Asset
{
    public class AssetQueryParameters
    {
        private const int MaxPageSize = 50;
        private int _pageSize = 10;

        public string? SearchTerm { get; set; }   // Name veya SerialNumber içinde arar
        public string? AssetType { get; set; }
        public string? Status { get; set; }
        public int PageNumber { get; set; } = 1;

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
        }
    }
}
