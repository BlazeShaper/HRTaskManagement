using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using HRTaskManagement.Application.DTOs.User;
using HRTaskManagement.Application.Interfaces;

namespace HRTaskManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("{id}/roles")]
        [Authorize(Policy = "RequireAdmin")]
        public async Task<IActionResult> AddRole(Guid id, [FromBody] UserRoleRequestDto request)
        {
            await _userService.AddRoleAsync(id, request);
            return NoContent();
        }

        [HttpDelete("{id}/roles")]
        [Authorize(Policy = "RequireAdmin")]
        public async Task<IActionResult> RemoveRole(Guid id, [FromBody] UserRoleRequestDto request)
        {
            await _userService.RemoveRoleAsync(id, request);
            return NoContent();
        }
    }
}