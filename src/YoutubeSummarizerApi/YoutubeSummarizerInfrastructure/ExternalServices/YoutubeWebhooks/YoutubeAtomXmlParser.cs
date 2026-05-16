using System.Xml.Linq;

namespace YoutubeSummarizer.Infrastructure.ExternalServices.YoutubeWebhooks
{
    public static class YoutubeAtomXmlParser
    {
        private static readonly XNamespace Yt = "http://www.youtube.com/xml/schemas/2015";
        private static readonly XNamespace Atom = "http://www.w3.org/2005/Atom";

        public static (string ChannelId, string VideoId) Parse(string xml)
        {
            var doc = XDocument.Parse(xml);
            var entry = doc.Root?.Element(Atom + "entry");

            var channelId = entry?.Element(Yt + "channelId")?.Value;
            var videoId = entry?.Element(Yt + "videoId")?.Value;

            if (string.IsNullOrEmpty(channelId) || string.IsNullOrEmpty(videoId))
                throw new InvalidOperationException("Failed to parse YouTube Atom XML: missing channelId or videoId.");

            return (channelId, videoId);
        }
    }
}
