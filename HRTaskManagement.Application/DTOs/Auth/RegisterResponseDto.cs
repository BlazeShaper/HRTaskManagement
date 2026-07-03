// Application/DTOs/Auth/RegisterResponseDto.cs
using System;

namespace HRTaskManagement.Application.DTOs.Auth
{
    public class RegisterResponseDto
    {
        public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string AssignedRole { get; set; } = string.Empty;
    }
}