// Application/DTOs/Department/DepartmentDto.cs
using System;

namespace HRTaskManagement.Application.DTOs.Department
{
    public class DepartmentDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int EmployeeCount { get; set; }
        public Guid? ManagerId { get; set; }
        public string? ManagerFullName { get; set; }
    }
}