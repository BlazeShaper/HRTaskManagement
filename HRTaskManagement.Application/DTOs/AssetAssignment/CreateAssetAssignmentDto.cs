namespace HRTaskManagement.Application.DTOs.AssetAssignment
{
    public class CreateAssetAssignmentDto
    {
        public Guid AssetId { get; set; }
        public Guid EmployeeId { get; set; }
        public string? Note { get; set; }
    }
}
