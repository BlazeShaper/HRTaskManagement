// Enums/AssetStatus.cs
namespace HRTaskManagement.Domain.Enums
{
    public enum AssetStatus
    {
        Available,     // Depoda, atanabilir
        Assigned,      // Şu an birine atanmış
        UnderRepair,   // Tamirde
        Retired        // Kullanımdan kaldırıldı
    }
}