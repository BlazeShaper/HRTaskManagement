// Application/Interfaces/ICurrentUserService.cs
namespace HRTaskManagement.Application.Interfaces
{
    public interface ICurrentUserService
    {
        Guid? UserId { get; }
        string? IpAddress { get; }
        IReadOnlyList<string> Roles { get; }
        bool IsInRole(string role);
    }
}