namespace HRTaskManagement.Application.DTOs.Asset
{
    public class AssetDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string AssetType { get; set; } = string.Empty;
        public string SerialNumber { get; set; } = string.Empty;
        public DateTime PurchaseDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
