// Application/Interfaces/IUserService.cs
using HRTaskManagement.Application.DTOs.User;

namespace HRTaskManagement.Application.Interfaces
{
    public interface IUserService
    {
        Task ChangeUserRoleAsync(Guid userId, ChangeUserRoleRequestDto request);
        Task AddRoleAsync(Guid userId, UserRoleRequestDto request);
        Task RemoveRoleAsync(Guid userId, UserRoleRequestDto request);
    }
}