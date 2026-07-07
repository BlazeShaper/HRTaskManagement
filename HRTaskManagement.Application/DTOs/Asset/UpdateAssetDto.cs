namespace HRTaskManagement.Application.DTOs.Asset
{
    public class UpdateAssetDto
    {
        public string Name { get; set; } = string.Empty;
        public string AssetType { get; set; } = string.Empty;
        public DateTime PurchaseDate { get; set; }
        public string Status { get; set; } = string.Empty; // "Available", "UnderRepair", "Retired"
    }
}
