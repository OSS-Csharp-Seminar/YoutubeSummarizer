using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YoutubeSummarizer.Application.Common.Models;
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
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto, CancellationToken cancellationToken)
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var result = await _authService.LoginAsync(dto, ip, cancellationToken);
            if (!result.Status)
                return Unauthorized(result);

            SetTokenCookies(result.Data!.AccessToken, result.Data.RefreshToken);

            var userInfo = new UserInfoResponseDto
            {
                UserId = Guid.Parse(result.Data.UserId),
                Email = result.Data.Email,
                FirstName = result.Data.FirstName,
                LastName = result.Data.LastName
            };

            return Ok(ServiceResponse<UserInfoResponseDto>.Success(userInfo, result.Message));
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto dto, CancellationToken cancellationToken)
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var result = await _authService.RegisterAsync(dto, ip, cancellationToken);
            if (!result.Status)
                return BadRequest(ServiceResponse<object>.Failure(result.Message));

            SetTokenCookies(result.Data!.AccessToken, result.Data.RefreshToken);

            var userInfo = new UserInfoResponseDto
            {
                UserId = Guid.Parse(result.Data.UserId),
                Email = result.Data.Email,
                FirstName = result.Data.FirstName,
                LastName = result.Data.LastName
            };

            return CreatedAtAction(nameof(Login), routeValues: null,
                value: ServiceResponse<UserInfoResponseDto>.Success(userInfo, result.Message));
        }

        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken(CancellationToken cancellationToken)
        {
            var accessToken = Request.Cookies["access_token"];
            var refreshToken = Request.Cookies["refresh_token"];

            if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshToken))
            {
                ClearTokenCookies();
                return Unauthorized(ServiceResponse<object>.Failure("No tokens provided."));
            }

            var dto = new RefreshTokenRequestDto { AccessToken = accessToken, RefreshToken = refreshToken };
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var result = await _authService.RefreshTokenAsync(dto, ip, cancellationToken);

            if (!result.Status)
            {
                ClearTokenCookies();
                return Unauthorized(result);
            }

            SetTokenCookies(result.Data!.AccessToken, result.Data.RefreshToken);
            return Ok(ServiceResponse<object>.Success(new { }, result.Message));
        }

        [HttpPost("logout")]
        [AllowAnonymous]
        public async Task<IActionResult> Logout(CancellationToken cancellationToken)
        {
            var refreshToken = Request.Cookies["refresh_token"];

            if (!string.IsNullOrEmpty(refreshToken))
            {
                var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                await _authService.LogoutAsync(refreshToken, ip, cancellationToken);
            }

            ClearTokenCookies();
            return NoContent();
        }

        [HttpGet("me")]
        public IActionResult Me()
        {
            var userInfo = new UserInfoResponseDto
            {
                UserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!),
                Email = User.FindFirstValue(ClaimTypes.Email)!,
                FirstName = User.FindFirstValue("FirstName")!,
                LastName = User.FindFirstValue("LastName")!
            };

            return Ok(ServiceResponse<UserInfoResponseDto>.Success(userInfo, "User info retrieved."));
        }

        private void SetTokenCookies(string accessToken, string refreshToken)
        {
            var isProduction = !HttpContext.RequestServices
                .GetRequiredService<IWebHostEnvironment>().IsDevelopment();

            Response.Cookies.Append("access_token", accessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = isProduction,
                SameSite = SameSiteMode.Lax,
                Path = "/api",
                MaxAge = TimeSpan.FromMinutes(15)
            });

            Response.Cookies.Append("refresh_token", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = isProduction,
                SameSite = SameSiteMode.Lax,
                Path = "/api/auth",
                MaxAge = TimeSpan.FromDays(30)
            });
        }

        private void ClearTokenCookies()
        {
            Response.Cookies.Delete("access_token", new CookieOptions { Path = "/api" });
            Response.Cookies.Delete("refresh_token", new CookieOptions { Path = "/api/auth" });
        }
    }
}
