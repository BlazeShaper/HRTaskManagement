// Application/DTOs/Task/CreateTaskDto.cs
namespace HRTaskManagement.Application.DTOs.Task
{
    public class CreateTaskDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid EmployeeId { get; set; }
        public DateTime? DueDate { get; set; }
    }
}