// Api/Middleware/ErrorResponse.cs
using System.Collections.Generic;

namespace HRTaskManagement.Api.Middleware
{
    public class ErrorResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<string>? Errors { get; set; }
    }
}