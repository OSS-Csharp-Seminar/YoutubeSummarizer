using System.Text.RegularExpressions;

namespace YoutubeSummarizer.Application.Features.YoutubeChannels
{
    public static class YoutubeChannelUrlParser
    {
        private static readonly Regex ChannelRegex = new(
            @"^https?://(www\.)?youtube\.com/(@[\w.-]+|channel/([\w-]+))",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static string ParseChannelIdentifier(string url)
        {
            var match = ChannelRegex.Match(url);
            if (!match.Success)
                throw new ArgumentException("Invalid YouTube channel URL.", nameof(url));

            var group2 = match.Groups[2].Value;
            var group3 = match.Groups[3].Value;

            return string.IsNullOrEmpty(group3) ? group2 : group3;
        }
    }
}
