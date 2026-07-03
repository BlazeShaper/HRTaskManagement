// Application/DTOs/Employee/CreateEmployeeDto.cs
using System;

namespace HRTaskManagement.Application.DTOs.Employee
{
    public class CreateEmployeeDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateOnly BirthDate { get; set; }
        public DateTime HireDate { get; set; }
        public Guid DepartmentId { get; set; }
        public Guid PositionId { get; set; }
        public string? ImageUrl { get; set; }
    }
}