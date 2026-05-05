using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YoutubeSummarizer.Application.DTOs;
using YoutubeSummarizer.Application.Interfaces;

namespace YoutubeSummarizer.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
            => _authService = authService;

        [HttpPost("login")]
        public async Task<IActionResult> Login(
            [FromBody] LoginRequestDto dto)
        {
            var result = await _authService.LoginAsync(dto);

            SetAuthCookie(result.AccessToken);

           
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(
            [FromBody] RegisterRequestDto dto)
        {
            var result = await _authService.RegisterAsync(dto);

            SetAuthCookie(result.AccessToken);

            return CreatedAtAction(
                actionName: nameof(Login),
                value: new
                {
                    result.UserId,
                    result.Email,
                    result.FullName
                });
        }

        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("access_token");
            return NoContent();
        }

        // ── Helper ────────────────────────────────────────────
        private void SetAuthCookie(string token)
        {
            Response.Cookies.Append("access_token", token, new CookieOptions
            {
                HttpOnly = true,   // JS ne može čitati
                Secure = true,   // samo HTTPS
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddHours(8)
            });
        }
    }
}
