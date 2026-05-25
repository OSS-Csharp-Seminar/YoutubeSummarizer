namespace YoutubeSummarizer.Application.Features.Notifications.Interfaces
{
    public interface INotificationHubService
    {
        Task NotifyUsersAsync(List<Guid> userIds, CancellationToken cancellationToken = default);
    }
}
