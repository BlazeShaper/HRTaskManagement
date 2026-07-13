// Application/DTOs/Task/UpdateTaskDto.cs
namespace HRTaskManagement.Application.DTOs.Task
{
    public class UpdateTaskDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime? DueDate { get; set; }
        public Guid EmployeeId { get; set; }
    }
}
