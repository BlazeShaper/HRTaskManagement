// Api/Controllers/AuthController.cs
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using HRTaskManagement.Application.DTOs.Auth;
using HRTaskManagement.Application.Interfaces;

namespace HRTaskManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var result = await _authService.LoginAsync(request);

            if (result == null)
                return Unauthorized(new { message = "Kullanıcı adı veya şifre hatalı." });

            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            try
            {
                var result = await _authService.RegisterAsync(request);
                return CreatedAtAction(nameof(Register), new { id = result.UserId }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}