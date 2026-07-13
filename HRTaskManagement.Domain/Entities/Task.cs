using HRTaskManagement.Domain.Common;
using HRTaskManagement.Domain.Enums;

namespace HRTaskManagement.Domain.Entities
{
    public class HrTask : BaseEntity
    {
        // Ana Özellikler
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TaskPriority Priority { get; set; }
        public Domain.Enums.TaskStatus Status { get; set; }

        public Guid CreatedById { get; set; }
        public Guid AssignedToId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime DueDate { get; set; }
        public DateTime? CompletedAt { get; set; }

        // Navigasyon Özellikleri (Navigation Properties)
        public Employee? CreatedBy { get; set; }
        public Employee? AssignedTo { get; set; }
        public ICollection<TaskComment> Comments { get; set; } = new List<TaskComment>();
    }
}