// Application/Interfaces/IAuthService.cs
using System.Threading.Tasks;
using HRTaskManagement.Application.DTOs.Auth;

namespace HRTaskManagement.Application.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto?> LoginAsync(LoginRequestDto request);
        Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto request);
        Task<LoginResponseDto> RefreshTokenAsync(string refreshToken);
        Task RevokeTokenAsync(string refreshToken);
        Task ChangePasswordAsync(Guid userId, ChangePasswordDto dto);
    }
}