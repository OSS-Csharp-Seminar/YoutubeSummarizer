using YoutubeSummarizer.Application.Features.YoutubeWebhooks.Models;

namespace YoutubeSummarizer.Application.Features.YoutubeWebhooks.Interfaces
{
    public interface IYoutubeWebhookNotificationParser
    {
        YoutubeWebhookNotification Parse(string payload);
    }
}
