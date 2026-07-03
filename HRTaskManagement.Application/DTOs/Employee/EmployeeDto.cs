// Application/DTOs/Employee/EmployeeDto.cs
using System;

namespace HRTaskManagement.Application.DTOs.Employee
{
    public class EmployeeDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateOnly BirthDate { get; set; }
        public DateTime HireDate { get; set; }
        public bool IsActive { get; set; }
        public string? ImageUrl { get; set; }

        // İlişkili verileri "düzleştirilmiş" (flattened) olarak taşıyoruz
        public Guid DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;

        public Guid PositionId { get; set; }
        public string PositionTitle { get; set; } = string.Empty;

        public string Username { get; set; } = string.Empty;
    }
}