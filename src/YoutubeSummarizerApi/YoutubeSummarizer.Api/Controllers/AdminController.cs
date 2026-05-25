using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YoutubeSummarizer.Application.Features.Admin.Dtos;
using YoutubeSummarizer.Application.Features.Admin.Interfaces;

namespace YoutubeSummarizer.Api.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpPost("notifications/global")]
        public async Task<IActionResult> CreateGlobalNotification(
            [FromBody] CreateGlobalNotificationRequest request,
            CancellationToken cancellationToken)
        {
            var senderName = User.FindFirstValue("FirstName") ?? "Admin";
            var result = await _adminService.SendGlobalNotificationAsync(
                request.Title, request.Content, senderName, cancellationToken);
            if (!result.Status)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers(CancellationToken cancellationToken)
        {
            var result = await _adminService.GetAllUsersAsync(cancellationToken);
            if (!result.Status)
                return BadRequest(result);
            return Ok(result);
        }
    }
}
