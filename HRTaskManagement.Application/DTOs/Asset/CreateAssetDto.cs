namespace HRTaskManagement.Application.DTOs.Asset
{
    public class CreateAssetDto
    {
        public string Name { get; set; } = string.Empty;
        public string AssetType { get; set; } = string.Empty;
        public string SerialNumber { get; set; } = string.Empty;
        public DateTime PurchaseDate { get; set; }
    }
}
