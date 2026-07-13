using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
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

        /// <summary>
        /// Kullanıcı girişi yapar ve JWT erişim token'ı ile yenileme (refresh) token'ını döner.
        /// </summary>
        /// <param name="request">Kullanıcı adı ve şifreyi içeren giriş talebi.</param>
        /// <returns>Erişim token'ı, sona erme tarihi ve kullanıcı adı.</returns>
        /// <response code="200">Giriş başarılı ve token'lar oluşturuldu.</response>
        /// <response code="401">Kullanıcı adı veya şifre hatalı.</response>
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

        /// <summary>
        /// Yeni bir kullanıcı hesabı kaydeder.
        /// </summary>
        /// <param name="request">Kaydedilecek kullanıcının bilgileri.</param>
        /// <returns>Yeni oluşturulan kullanıcı kaydı bilgileri.</returns>
        /// <response code="201">Kayıt başarıyla oluşturuldu.</response>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            var result = await _authService.RegisterAsync(request);
            return CreatedAtAction(nameof(Register), new { id = result.UserId }, result);
        }

        /// <summary>
        /// Yenileme token'ı (refresh token) aracılığıyla yeni bir erişim token'ı talep eder.
        /// </summary>
        /// <returns>Yeni erişim token'ı ve sona erme tarihi.</returns>
        /// <response code="200">Yeni erişim token'ı başarıyla üretildi.</response>
        /// <response code="401">Geçerli bir yenileme token'ı bulunamadı veya yetkisiz işlem.</response>
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

        /// <summary>
        /// Kullanıcının oturumunu sonlandırır ve yenileme token'ını iptal eder.
        /// </summary>
        /// <returns>Boş içerik.</returns>
        /// <response code="204">Oturum başarıyla sonlandırıldı.</response>
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

        /// <summary>
        /// Giriş yapmış mevcut kullanıcının şifresini değiştirir.
        /// </summary>
        /// <param name="dto">Eski ve yeni şifre bilgilerini içeren veri nesnesi.</param>
        /// <returns>Boş içerik.</returns>
        /// <response code="204">Şifre başarıyla güncellendi.</response>
        /// <response code="401">Kullanıcı oturumu açılmamış veya yetkisiz erişim.</response>
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