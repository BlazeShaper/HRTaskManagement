using HRTaskManagement.Domain.Common;

namespace HRTaskManagement.Domain.Entities
{
    public class TaskComment : BaseEntity
    {
        public Guid TaskId { get; set; }
        public Guid EmployeeId { get; set; }
        public string Comment { get; set; } = string.Empty;

        // Navigation Properties
        public TaskItem? Task { get; set; }
        public Employee? Employee { get; set; }
    }
}