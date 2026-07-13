// Persistence/Services/UserService.cs
using Microsoft.EntityFrameworkCore;
using HRTaskManagement.Application.DTOs.User;
using HRTaskManagement.Application.Interfaces;
using HRTaskManagement.Domain.Entities;
using HRTaskManagement.Persistence.Context;

namespace HRTaskManagement.Persistence.Services
{
    public class UserService : IUserService
    {
        private readonly WorkSphereDbContext _context;

        public UserService(WorkSphereDbContext context)
        {
            _context = context;
        }

        public async Task ChangeUserRoleAsync(Guid userId, ChangeUserRoleRequestDto request)
        {
            // 1. Kullanıcıyı, mevcut rolleriyle birlikte bul
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new KeyNotFoundException("Kullanıcı bulunamadı.");

            // 2. Yeni rol gerçekten var mı kontrol et
            var newRole = await _context.Roles
                .FirstOrDefaultAsync(r => r.Name == request.NewRoleName);

            if (newRole == null)
                throw new InvalidOperationException($"'{request.NewRoleName}' adında bir rol bulunamadı.");

            // 3. Kullanıcının mevcut tüm rollerini kaldır
            _context.UserRoles.RemoveRange(user.UserRoles);

            // 4. Yeni rolü ata
            user.UserRoles.Add(new UserRole
            {
                UserId = user.Id,
                RoleId = newRole.Id
            });

            await _context.SaveChangesAsync();
        }

        public async Task AddRoleAsync(Guid userId, UserRoleRequestDto request)
        {
            // 1. Kullanıcıyı bul
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new KeyNotFoundException("Kullanıcı bulunamadı.");

            // 2. Rol var mı kontrol et
            var role = await _context.Roles
                .FirstOrDefaultAsync(r => r.Name == request.RoleName);

            if (role == null)
                throw new KeyNotFoundException($"'{request.RoleName}' adında bir rol bulunamadı.");

            // 3. Kullanıcıda bu rol zaten var mı?
            if (user.UserRoles.Any(ur => ur.RoleId == role.Id))
                throw new InvalidOperationException("Kullanıcı zaten bu role sahip.");

            // 4. Rolü ekle
            _context.UserRoles.Add(new UserRole
            {
                UserId = user.Id,
                RoleId = role.Id
            });

            await _context.SaveChangesAsync();
        }

        public async Task RemoveRoleAsync(Guid userId, UserRoleRequestDto request)
        {
            // 1. Kullanıcıyı bul
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new KeyNotFoundException("Kullanıcı bulunamadı.");

            // 2. Rol var mı kontrol et
            var role = await _context.Roles
                .FirstOrDefaultAsync(r => r.Name == request.RoleName);

            if (role == null)
                throw new KeyNotFoundException($"'{request.RoleName}' adında bir rol bulunamadı.");

            // 3. Kullanıcıda bu rol var mı?
            var userRole = user.UserRoles.FirstOrDefault(ur => ur.RoleId == role.Id);
            if (userRole == null)
                throw new InvalidOperationException("Kullanıcı bu role sahip değil.");

            // 4. Rolü kaldır
            _context.UserRoles.Remove(userRole);

            await _context.SaveChangesAsync();
        }
    }
}