// Application/Interfaces/ICurrentUserService.cs
using System;
using System.Collections.Generic;

namespace HRTaskManagement.Application.Interfaces
{
    public interface ICurrentUserService
    {
        Guid? UserId { get; }
        IReadOnlyList<string> Roles { get; }
        bool IsInRole(string role);
    }
}