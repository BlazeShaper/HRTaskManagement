using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using HRTaskManagement.Application.DTOs.Auth;
using HRTaskManagement.Application.Interfaces;
using HRTaskManagement.Api.Attributes;

namespace HRTaskManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ICurrentUserService _currentUserService;

        public AuthController(IAuthService authService, ICurrentUserService currentUserService)
        {
            _authService = authService;
            _currentUserService = currentUserService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var result = await _authService.LoginAsync(request);

            if (result == null)
                return Unauthorized(new { message = "Kullanıcı adı veya şifre hatalı." });

            SetRefreshTokenCookie(result.RefreshToken);

            return Ok(new
            {
                result.AccessToken,
                result.AccessTokenExpiration,
                result.Username
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            var result = await _authService.RegisterAsync(request);
            return CreatedAtAction(nameof(Register), new { id = result.UserId }, result);
        }

        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> Refresh()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized(new { message = "Refresh token bulunamadı." });

            var result = await _authService.RefreshTokenAsync(refreshToken);

            SetRefreshTokenCookie(result.RefreshToken);

            return Ok(new
            {
                result.AccessToken,
                result.AccessTokenExpiration,
                result.Username
            });
        }

        [HttpPost("logout")]
        [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            if (!string.IsNullOrEmpty(refreshToken))
            {
                await _authService.RevokeTokenAsync(refreshToken);
            }

            Response.Cookies.Delete("refreshToken", new CookieOptions
            {
                Path = "/",
                Secure = true,
                SameSite = SameSiteMode.Strict
            });

            return NoContent();
        }

        [HttpPost("change-password")]
        [Authorize]
        [AllowPasswordChange]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var userId = _currentUserService.UserId
                ?? throw new UnauthorizedAccessException("Kullanıcı kimliği bulunamadı.");
            await _authService.ChangePasswordAsync(userId, dto);
            return NoContent();
        }

        private void SetRefreshTokenCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(7),
                Path = "/"
            };

            Response.Cookies.Append("refreshToken", token, cookieOptions);
        }
    }
}