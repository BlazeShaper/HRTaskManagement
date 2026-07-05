// Application/DTOs/Department/CreateDepartmentDto.cs
using System;

namespace HRTaskManagement.Application.DTOs.Department
{
    public class CreateDepartmentDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid? ManagerId { get; set; }
    }
}