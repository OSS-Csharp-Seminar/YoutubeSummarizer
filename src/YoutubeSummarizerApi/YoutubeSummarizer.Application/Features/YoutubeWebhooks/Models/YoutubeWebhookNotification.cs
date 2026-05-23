namespace YoutubeSummarizer.Application.Features.YoutubeWebhooks.Models
{
    public sealed record YoutubeWebhookNotification(string ChannelId, string VideoId);
}
