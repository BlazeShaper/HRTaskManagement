// Infrastructure/Services/PasswordHasher.cs
using HRTaskManagement.Application.Interfaces;

namespace HRTaskManagement.Infrastructure.Services
{
    public class PasswordHasher : IPasswordHasher
    {
        public string HashPassword(string password)
        {
            // WorkFactor 12: güvenlik ile performans arasında dengeli bir değer
            return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}