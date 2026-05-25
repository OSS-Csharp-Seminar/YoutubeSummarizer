using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YoutubeSummarizer.Application.Features.Notifications.Dtos;
using YoutubeSummarizer.Application.Features.Notifications.Interfaces;

namespace YoutubeSummarizer.Api.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public AdminController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpPost("notifications/global")]
        public async Task<IActionResult> CreateGlobalNotification(
            [FromBody] CreateGlobalNotificationRequest request,
            CancellationToken cancellationToken)
        {
            var senderName = User.FindFirstValue("FirstName") ?? "Admin";
            var result = await _notificationService.CreateGlobalNotificationAsync(request, senderName, cancellationToken);
            if (!result.Status)
                return BadRequest(result);
            return Ok(result);
        }
    }
}
