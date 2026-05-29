using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YoutubeSummarizer.Application.Features.Notifications.Dtos;
using YoutubeSummarizer.Application.Features.Notifications.Interfaces;

namespace YoutubeSummarizer.Api.Controllers
{
    [ApiController]
    [Route("api/notifications")]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetNotifications(
            [FromQuery] GetNotificationsQuery query,
            CancellationToken cancellationToken = default)
        {
            var result = await _notificationService.GetNotificationsAsync(query, cancellationToken);
            if (!result.Status)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount(CancellationToken cancellationToken)
        {
            var result = await _notificationService.GetUnreadCountAsync(cancellationToken);
            if (!result.Status)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPut("{id:guid}/read")]
        public async Task<IActionResult> MarkAsRead(Guid id, CancellationToken cancellationToken)
        {
            var result = await _notificationService.MarkAsReadAsync(id, cancellationToken);
            if (!result.Status)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPut("read-all")]
        public async Task<IActionResult> MarkAllAsRead(CancellationToken cancellationToken)
        {
            var result = await _notificationService.MarkAllAsReadAsync(cancellationToken);
            if (!result.Status)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPut("{id:guid}/dismiss")]
        public async Task<IActionResult> Dismiss(Guid id, CancellationToken cancellationToken)
        {
            var result = await _notificationService.DismissAsync(id, cancellationToken);
            if (!result.Status)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpDelete("dismiss-all")]
        public async Task<IActionResult> DismissAll(CancellationToken cancellationToken)
        {
            var result = await _notificationService.DismissAllAsync(cancellationToken);
            if (!result.Status)
                return BadRequest(result);
            return Ok(result);
        }
    }
}
