// Persistence/Services/AuthService.cs
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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