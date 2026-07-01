// Entities/TaskItem.cs
using HRTaskManagement.Domain.Enums;

namespace HRTaskManagement.Domain.Entities
{
    public class TaskItem : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid EmployeeId { get; set; }
        public Domain.Enums.TaskStatus Status { get; set; }
        public DateTime? DueDate { get; set; }

        public Employee? Employee { get; set; }
    }
}