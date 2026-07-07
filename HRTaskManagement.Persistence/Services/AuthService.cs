// Persistence/Services/AuthService.cs
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using HRTaskManagement.Application.DTOs.Auth;
using HRTaskManagement.Application.Interfaces;
using HRTaskManagement.Domain.Entities;
using HRTaskManagement.Persistence.Context;

namespace HRTaskManagement.Persistence.Services
{
    public class AuthService : IAuthService
    {
        private readonly WorkSphereDbContext _context;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtService _jwtService;
        private readonly IConfiguration _configuration;

        public AuthService(
            WorkSphereDbContext context,
            IPasswordHasher passwordHasher,
            IJwtService jwtService,
            IConfiguration configuration)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
            _configuration = configuration;
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
            var refreshTokenValue = _jwtService.GenerateRefreshToken();

            // Refresh token geçerlilik süresini konfigürasyondan oku (varsayılan 7 gün)
            var expiryDaysStr = _configuration["JwtSettings:RefreshTokenExpiryDays"] 
                ?? _configuration["JwtSettings:RefreshTokenExpirationDays"] 
                ?? "7";
            int expiryDays = int.TryParse(expiryDaysStr, out var parsedDays) ? parsedDays : 7;

            // Önceki aktif token'ları iptal et (güvenlik için iyi bir önlem)
            await RevokeAllActiveTokensForUserAsync(user.Id);

            var refreshToken = new RefreshToken
            {
                Token = refreshTokenValue,
                ExpiresAt = DateTime.UtcNow.AddDays(expiryDays),
                UserId = user.Id
            };

            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();

            var expirationMinutesStr = _configuration["JwtSettings:AccessTokenExpirationMinutes"] ?? "15";
            int expirationMinutes = int.TryParse(expirationMinutesStr, out var parsedMinutes) ? parsedMinutes : 15;

            return new LoginResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshTokenValue,
                AccessTokenExpiration = DateTime.UtcNow.AddMinutes(expirationMinutes),
                Username = user.Username
            };
        }

        public async Task<LoginResponseDto> RefreshTokenAsync(string refreshTokenValue)
        {
            var storedToken = await _context.RefreshTokens
                .Include(rt => rt.User)
                    .ThenInclude(u => u.UserRoles)
                        .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(rt => rt.Token == refreshTokenValue);

            if (storedToken is null)
                throw new UnauthorizedAccessException("Geçersiz refresh token.");

            if (storedToken.IsRevoked)
            {
                // REUSE DETECTION: İptal edilmiş bir token tekrar kullanılmaya çalışılıyor.
                // Bu, token'ın çalınmış olabileceğinin göstergesi -> tüm oturumları kapat.
                await RevokeAllActiveTokensForUserAsync(storedToken.UserId);
                await _context.SaveChangesAsync();
                throw new UnauthorizedAccessException(
                    "Bu refresh token daha önce kullanılmış. Güvenlik nedeniyle tüm oturumlar sonlandırıldı, lütfen tekrar giriş yapın.");
            }

            if (storedToken.IsExpired)
                throw new UnauthorizedAccessException("Refresh token süresi dolmuş, lütfen tekrar giriş yapın.");

            // Rotation: eskisini iptal et, yenisini üret
            storedToken.RevokedAt = DateTime.UtcNow;
            
            var expiryDaysStr = _configuration["JwtSettings:RefreshTokenExpiryDays"] 
                ?? _configuration["JwtSettings:RefreshTokenExpirationDays"] 
                ?? "7";
            int expiryDays = int.TryParse(expiryDaysStr, out var parsedDays) ? parsedDays : 7;

            var newRefreshTokenValue = _jwtService.GenerateRefreshToken();
            var newRefreshToken = new RefreshToken
            {
                Token = newRefreshTokenValue,
                ExpiresAt = DateTime.UtcNow.AddDays(expiryDays),
                UserId = storedToken.UserId
            };

            storedToken.ReplacedByToken = newRefreshTokenValue;

            _context.RefreshTokens.Add(newRefreshToken);
            await _context.SaveChangesAsync();

            var newAccessToken = _jwtService.GenerateAccessToken(storedToken.User);

            var expirationMinutesStr = _configuration["JwtSettings:AccessTokenExpirationMinutes"] ?? "15";
            int expirationMinutes = int.TryParse(expirationMinutesStr, out var parsedMinutes) ? parsedMinutes : 15;

            return new LoginResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshTokenValue,
                AccessTokenExpiration = DateTime.UtcNow.AddMinutes(expirationMinutes),
                Username = storedToken.User.Username
            };
        }

        public async Task RevokeTokenAsync(string refreshTokenValue)
        {
            var storedToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == refreshTokenValue);

            if (storedToken == null)
                throw new KeyNotFoundException("Refresh token bulunamadı.");

            if (storedToken.RevokedAt == null)
            {
                storedToken.RevokedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task ChangePasswordAsync(Guid userId, ChangePasswordDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId)
                ?? throw new KeyNotFoundException("Kullanıcı bulunamadı.");

            if (!_passwordHasher.VerifyPassword(dto.CurrentPassword, user.PasswordHash))
                throw new UnauthorizedAccessException("Mevcut şifre yanlış.");

            user.PasswordHash = _passwordHasher.HashPassword(dto.NewPassword);
            user.MustChangePassword = false;
            user.UpdatedDate = DateTime.UtcNow;

            // Güvenlik: şifre değiştiğinde tüm eski oturumlar geçersiz olsun
            await RevokeAllActiveTokensForUserAsync(userId);

            await _context.SaveChangesAsync();
        }

        private async Task RevokeAllActiveTokensForUserAsync(Guid userId)
        {
            var activeTokens = await _context.RefreshTokens
                .Where(rt => rt.UserId == userId && rt.RevokedAt == null && rt.ExpiresAt > DateTime.UtcNow)
                .ToListAsync();

            foreach (var token in activeTokens)
            {
                token.RevokedAt = DateTime.UtcNow;
            }
        }

        public async Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto request)
        {
            // 1. Kullanıcı adı kontrolü
            if (await _context.Users.AnyAsync(u => u.Username == request.Username))
            {
                throw new InvalidOperationException("Kullanıcı adı zaten kullanımda.");
            }

            // 2. E-posta kontrolü
            if (await _context.Employees.AnyAsync(e => e.Email == request.Email))
            {
                throw new InvalidOperationException("E-posta adresi zaten kullanımda.");
            }

            // 3. Rol kontrolü
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == request.Role);
            if (role == null)
            {
                throw new InvalidOperationException("Belirtilen rol sistemde mevcut değil.");
            }

            // 4. Varsayılan Departman ve Pozisyon kontrolü/oluşturulması
            var department = await _context.Departments.FirstOrDefaultAsync();
            if (department == null)
            {
                department = new Department { Name = "Genel Departman" };
                _context.Departments.Add(department);
            }

            var position = await _context.Positions.FirstOrDefaultAsync();
            if (position == null)
            {
                position = new Position { Title = "Çalışan" };
                _context.Positions.Add(position);
            }

            // 5. Yeni kullanıcı oluşturma
            var user = new User
            {
                Username = request.Username,
                PasswordHash = _passwordHasher.HashPassword(request.Password),
                IsActive = true
            };
            _context.Users.Add(user);

            // 6. Rol atama
            var userRole = new UserRole
            {
                User = user,
                Role = role
            };
            _context.UserRoles.Add(userRole);

            // 7. Çalışan/Employee kaydı oluşturma
            var employee = new Employee
            {
                User = user,
                FirstName = request.Username,
                LastName = "Kullanıcısı",
                Email = request.Email,
                Phone = string.Empty,
                BirthDate = new DateOnly(1990, 1, 1),
                HireDate = DateTime.UtcNow,
                Department = department,
                Position = position,
                IsActive = true
            };
            _context.Employees.Add(employee);

            // 8. Değişiklikleri kaydet
            await _context.SaveChangesAsync();

            return new RegisterResponseDto
            {
                UserId = user.Id,
                Username = user.Username,
                Email = employee.Email,
                AssignedRole = role.Name
            };
        }
    }
}