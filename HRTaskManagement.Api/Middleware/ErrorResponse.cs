// Api/Middleware/ErrorResponse.cs
namespace HRTaskManagement.Api.Middleware
{
    public class ErrorResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<string>? Errors { get; set; }
    }
}