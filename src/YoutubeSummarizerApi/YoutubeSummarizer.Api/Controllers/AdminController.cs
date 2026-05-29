using System;
using System.Threading;
using System.Threading.Tasks;
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

        [HttpPost("users/{id}/toggle-ban")]
        public async Task<IActionResult> ToggleBan(Guid id, CancellationToken cancellationToken)
        {
            var result = await _adminService.ToggleBanAsync(id, cancellationToken);
            if (!result.Status)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("users/{id}/logout")]
        public async Task<IActionResult> LogOutUser(Guid id, CancellationToken cancellationToken)
        {
            var result = await _adminService.LogOutUserAsync(id, cancellationToken);
            if (!result.Status)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpDelete("subscriptions/{id}")]
        public async Task<IActionResult> CancelSubscription(Guid id, CancellationToken cancellationToken)
        {
            var result = await _adminService.CancelSubscriptionAsync(id, cancellationToken);
            if (!result.Status)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("mock-webhook")]
        public async Task<IActionResult> MockWebhook(
            [FromBody] MockWebhookRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _adminService.MockWebhookAsync(request.VideoUrl, cancellationToken);
            if (!result.Status)
                return BadRequest(result);
            return Ok(result);
        }
    }
}
