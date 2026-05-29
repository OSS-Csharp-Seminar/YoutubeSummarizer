using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace YoutubeSummarizer.Application.Features.Notifications.Interfaces
{
    public interface INotificationHubService
    {
        Task NotifyUsersAsync(List<Guid> userIds, CancellationToken cancellationToken = default);
    }
}
