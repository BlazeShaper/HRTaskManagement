// Persistence/Services/AuthService.cs
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HRTaskManagement.Application.DTOs.Auth;
using HRTaskManagement.Application.Interfaces;
using HRTaskManagement.Persistence.Context;

namespace HRTaskManagement.Persistence.Services
{
    public class AuthService : IAuthService
    {
        private readonly WorkSphereDbContext _context;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtService _jwtService;

        public AuthService(
            WorkSphereDbContext context,
            IPasswordHasher passwordHasher,
            IJwtService jwtService)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
        {
            // Kullanıcıyı, rollerini de yükleyerek bul
            var user = await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Username == request.Username);

            // Kullanıcı yoksa VEYA aktif değilse giriş reddedilir
            if (user == null || !user.IsActive)
                return null;

            // Şifre eşleşmiyor mu?
            bool isPasswordValid = _passwordHasher.VerifyPassword(request.Password, user.PasswordHash);
            if (!isPasswordValid)
                return null;

            // Token'ları üret
            var accessToken = _jwtService.GenerateAccessToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();

            return new LoginResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AccessTokenExpiration = System.DateTime.UtcNow.AddMinutes(15),
                Username = user.Username
            };
        }
    }
}