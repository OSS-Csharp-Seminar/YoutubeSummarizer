using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using YoutubeSummarizer.Application.Features.Notifications.Interfaces;

namespace YoutubeSummarizer.Infrastructure.Hubs
{
    public class NotificationHubService : INotificationHubService
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationHubService(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotifyUsersAsync(List<Guid> userIds, CancellationToken cancellationToken = default)
        {
            foreach (var userId in userIds)
            {
                await _hubContext.Clients.Group(userId.ToString())
                    .SendAsync("NotificationReceived", cancellationToken: cancellationToken);
            }
        }
    }
}
