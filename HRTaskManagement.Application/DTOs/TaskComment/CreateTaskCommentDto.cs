namespace HRTaskManagement.Application.DTOs.TaskComment
{
    public class CreateTaskCommentDto
    {
        public Guid TaskId { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
}
