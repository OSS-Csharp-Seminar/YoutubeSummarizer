using System;
using System.Linq;
using System.Xml.Linq;
using YoutubeSummarizer.Application.Features.YoutubeWebhooks.Interfaces;
using YoutubeSummarizer.Application.Features.YoutubeWebhooks.Models;

namespace YoutubeSummarizer.Infrastructure.ExternalServices.YoutubeWebhooks
{
    public class YoutubeAtomXmlParser : IYoutubeWebhookNotificationParser
    {
        private static readonly XNamespace Yt = "http://www.youtube.com/xml/schemas/2015";
        private static readonly XNamespace Atom = "http://www.w3.org/2005/Atom";

        public YoutubeWebhookNotification Parse(string payload)
        {
            var doc = XDocument.Parse(payload);
            var entry = doc.Root?.Element(Atom + "entry");

            var channelId = entry?.Element(Yt + "channelId")?.Value;
            var videoId = entry?.Element(Yt + "videoId")?.Value;

            if (string.IsNullOrEmpty(channelId) || string.IsNullOrEmpty(videoId))
                throw new InvalidOperationException("Failed to parse YouTube Atom XML: missing channelId or videoId.");

            var videoTitle = entry?.Element(Atom + "title")?.Value ?? "Unknown";
            var channelName = entry?.Element(Atom + "author")?.Element(Atom + "name")?.Value ?? "Unknown";
            var videoUrl = entry?.Elements(Atom + "link")
                .FirstOrDefault(l => l.Attribute("rel")?.Value == "alternate")
                ?.Attribute("href")?.Value ?? $"https://www.youtube.com/watch?v={videoId}";

            DateTime? publishedAtUtc = null;
            var publishedStr = entry?.Element(Atom + "published")?.Value;
            if (DateTime.TryParse(publishedStr, out var parsed))
                publishedAtUtc = parsed.ToUniversalTime();

            return new YoutubeWebhookNotification(channelId, videoId, videoTitle, channelName, videoUrl, publishedAtUtc);
        }
    }
}
