namespace HRTaskManagement.Application.DTOs.AssetAssignment
{
    public class AssetAssignmentDto
    {
        public Guid Id { get; set; }
        public Guid AssetId { get; set; }
        public string AssetName { get; set; } = string.Empty;
        public string AssetSerialNumber { get; set; } = string.Empty;
        public Guid EmployeeId { get; set; }
        public string EmployeeFullName { get; set; } = string.Empty;
        public Guid AssignedByUserId { get; set; }
        public string AssignedByUsername { get; set; } = string.Empty;
        public DateTime AssignedDate { get; set; }
        public DateTime? ReturnedDate { get; set; }
        public string? Note { get; set; }
        public bool IsActive => ReturnedDate == null;
    }
}
