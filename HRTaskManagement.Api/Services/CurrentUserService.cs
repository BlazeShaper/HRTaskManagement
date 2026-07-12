// Api/Services/CurrentUserService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using HRTaskManagement.Application.Interfaces;

namespace HRTaskManagement.Api.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid? UserId
        {
            get
            {
                var userIdClaim = _httpContextAccessor.HttpContext?.User?
                    .FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)
                    ?? _httpContextAccessor.HttpContext?.User?
                    .FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);

                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
                    return null;

                return userId;
            }
        }

        public string? IpAddress
        {
            get
            {
                return _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();
            }
        }

        public IReadOnlyList<string> Roles
        {
            get
            {
                return _httpContextAccessor.HttpContext?.User
                    .FindAll(ClaimTypes.Role)
                    .Select(c => c.Value)
                    .ToList() ?? new List<string>();
            }
        }

        public bool IsInRole(string role)
        {
            return _httpContextAccessor.HttpContext?.User.IsInRole(role) ?? false;
        }
    }
}