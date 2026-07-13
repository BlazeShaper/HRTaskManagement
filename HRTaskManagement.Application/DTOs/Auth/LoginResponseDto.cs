// Application/DTOs/Auth/LoginResponseDto.cs
namespace HRTaskManagement.Application.DTOs.Auth
{
	public class LoginResponseDto
	{
		public string AccessToken { get; set; } = string.Empty;
		public string RefreshToken { get; set; } = string.Empty;
		public DateTime AccessTokenExpiration { get; set; }
		public string Username { get; set; } = string.Empty;
	}
}