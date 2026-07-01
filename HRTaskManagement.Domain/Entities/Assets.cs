using System;
using System.Collections.Generic;
using HRTaskManagement.Domain.Common;
using HRTaskManagement.Domain.Enums;

namespace HRTaskManagement.Domain.Entities
{
    public class Asset : BaseEntity
    {
        public string Name { get; set; } = string.Empty;         // Örn: "Dell Latitude 5420"
        public string AssetType { get; set; } = string.Empty;    // Örn: "Laptop", "Telefon"
        public string SerialNumber { get; set; } = string.Empty;
        public DateTime PurchaseDate { get; set; }
        public AssetStatus Status { get; set; } = AssetStatus.Available;

        public ICollection<AssetAssignment> AssetAssignments { get; set; } = new List<AssetAssignment>();
    }
}