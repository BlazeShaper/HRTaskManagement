namespace HRTaskManagement.Application.DTOs.Employee
{
    public class CreateEmployeeResultDto
    {
        public EmployeeDto Employee { get; set; } = null!;
        public string TemporaryPassword { get; set; } = string.Empty;
    }
}