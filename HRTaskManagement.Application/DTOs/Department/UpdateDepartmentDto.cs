// Application/DTOs/Department/UpdateDepartmentDto.cs
namespace HRTaskManagement.Application.DTOs.Department
{
    public class UpdateDepartmentDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid? ManagerId { get; set; }
    }
}