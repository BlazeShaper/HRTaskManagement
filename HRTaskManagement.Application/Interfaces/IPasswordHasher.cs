// Application/Interfaces/IPasswordHasher.cs
namespace HRTaskManagement.Application.Interfaces
{
    public interface IPasswordHasher
    {
        string HashPassword(string password);
        bool VerifyPassword(string password, string hashedPassword);
    }
}