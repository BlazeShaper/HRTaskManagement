// Application/Interfaces/IJwtService.cs
using System;
using HRTaskManagement.Domain.Entities;

namespace HRTaskManagement.Application.Interfaces
{
    public interface IJwtService
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();
    }
}