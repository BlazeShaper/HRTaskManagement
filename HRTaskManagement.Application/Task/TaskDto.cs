// Application/DTOs/Task/TaskDto.cs
using System;

namespace HRTaskManagement.Application.DTOs.Task
{
    public class TaskDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime? DueDate { get; set; }

        public Guid EmployeeId { get; set; }
        public string EmployeeFullName { get; set; } = string.Empty;

        public int CommentCount { get; set; }
    }
}