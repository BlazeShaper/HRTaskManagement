namespace HRTaskManagement.Application.DTOs.Position
{
    public class CreatePositionDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}