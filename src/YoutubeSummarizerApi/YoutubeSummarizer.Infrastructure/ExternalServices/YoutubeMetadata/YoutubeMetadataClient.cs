using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using YoutubeSummarizer.Application.Common.Interfaces;
using YoutubeSummarizer.Application.Common.Models;

namespace YoutubeSummarizer.Infrastructure.ExternalServices.YoutubeMetadata
{
    public class YoutubeMetadataClient : IYoutubeMetadataClient
    {
        private readonly HttpClient _httpClient;
        private static readonly Regex VideoIdRegex = new(@"(?:v=|youtu\.be/|/embed/|/v/)([a-zA-Z0-9_-]{11})", RegexOptions.Compiled);
        private static readonly Regex ChannelIdRegex = new(@"""channelId""\s*:\s*""(UC[a-zA-Z0-9_-]{22})""", RegexOptions.Compiled);
        private static readonly Regex CanonicalChannelRegex = new(@"<link\s+rel=""canonical""\s+href=""https://www\.youtube\.com/channel/(UC[a-zA-Z0-9_-]{22})""", RegexOptions.Compiled);
        private static readonly Regex ChannelNameRegex = new(@"""name""\s*:\s*""([^""]+)""", RegexOptions.Compiled);
        private static readonly Regex OgTitleRegex = new(@"<meta\s+property=""og:title""\s+content=""([^""]+)""", RegexOptions.Compiled);

        public YoutubeMetadataClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; Googlebot/2.1; +http://www.google.com/bot.html)");
        }

        public async Task<YoutubeVideoMetadata> GetVideoMetadataAsync(string videoUrl, CancellationToken cancellationToken = default)
        {
            var videoId = ExtractVideoId(videoUrl);
            if (string.IsNullOrEmpty(videoId))
                throw new ArgumentException("Could not extract video ID from URL.");

            var oembedUrl = $"https://www.youtube.com/oembed?url=https://www.youtube.com/watch?v={videoId}&format=json";
            var oembed = await _httpClient.GetFromJsonAsync<OEmbedResponse>(oembedUrl, cancellationToken)
                ?? throw new InvalidOperationException("Failed to fetch oEmbed data.");

            var channelId = await ExtractChannelIdAsync(oembed.AuthorUrl, cancellationToken);

            return new YoutubeVideoMetadata
            {
                VideoId = videoId,
                ChannelId = channelId,
                Title = oembed.Title,
                ChannelName = oembed.AuthorName
            };
        }

        public async Task<YoutubeChannelMetadata> GetChannelMetadataAsync(string channelUrl, CancellationToken cancellationToken = default)
        {
            var html = await _httpClient.GetStringAsync(channelUrl, cancellationToken);

            var channelId = ExtractChannelIdFromHtml(html);
            var channelName = ExtractChannelNameFromHtml(html);

            return new YoutubeChannelMetadata
            {
                ChannelId = channelId,
                ChannelName = channelName
            };
        }

        private static string? ExtractVideoId(string url)
        {
            var match = VideoIdRegex.Match(url);
            return match.Success ? match.Groups[1].Value : null;
        }

        private async Task<string> ExtractChannelIdAsync(string channelPageUrl, CancellationToken cancellationToken)
        {
            var html = await _httpClient.GetStringAsync(channelPageUrl, cancellationToken);
            return ExtractChannelIdFromHtml(html);
        }

        private static string ExtractChannelIdFromHtml(string html)
        {
            var match = ChannelIdRegex.Match(html);
            if (match.Success)
                return match.Groups[1].Value;

            match = CanonicalChannelRegex.Match(html);
            if (match.Success)
                return match.Groups[1].Value;

            throw new InvalidOperationException("Could not extract channel ID from channel page.");
        }

        private static string ExtractChannelNameFromHtml(string html)
        {
            var match = OgTitleRegex.Match(html);
            if (match.Success)
                return match.Groups[1].Value;

            match = ChannelNameRegex.Match(html);
            if (match.Success)
                return match.Groups[1].Value;

            return "Unknown";
        }

        private class OEmbedResponse
        {
            [JsonPropertyName("title")]
            public string Title { get; set; } = string.Empty;

            [JsonPropertyName("author_name")]
            public string AuthorName { get; set; } = string.Empty;

            [JsonPropertyName("author_url")]
            public string AuthorUrl { get; set; } = string.Empty;
        }
    }
}
