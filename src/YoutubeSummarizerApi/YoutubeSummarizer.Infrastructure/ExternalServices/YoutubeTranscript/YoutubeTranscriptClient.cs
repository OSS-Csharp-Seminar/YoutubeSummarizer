using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using YoutubeSummarizer.Application.Common.Interfaces;
using YoutubeSummarizer.Application.Features.YoutubeTranscript.Dtos;
using YoutubeSummarizer.Application.Features.YoutubeTranscript.Interfaces;
using YoutubeSummarizer.Infrastructure.ExternalServices.YoutubeTranscript.Models;

namespace YoutubeSummarizer.Infrastructure.ExternalServices.YoutubeTranscript
{
    public class YoutubeTranscriptClient : IYoutubeTranscriptClient
    {
        private readonly HttpClient _httpClient;
        private readonly IApiSettingsRepository _apiSettingsRepository;

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public YoutubeTranscriptClient(HttpClient httpClient, IApiSettingsRepository apiSettingsRepository)
        {
            _httpClient = httpClient;
            _apiSettingsRepository = apiSettingsRepository;
        }

        public async Task<GetYoutubeTranscriptResponse> GetTranscriptAsync(GetYoutubeTranscriptRequest request, CancellationToken cancellationToken = default)
        {
            var apiKey = await _apiSettingsRepository.GetApiKeyAsync("YoutubeTranscriptApi", cancellationToken);

            var queryString = $"?video_url={Uri.EscapeDataString(request.VideoUrl)}&send_metadata=true&format=json&include_timestamp=true";

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"/api/v2/youtube/transcript{queryString}");
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            var response = await _httpClient.SendAsync(requestMessage, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw response.StatusCode switch
                {
                    HttpStatusCode.BadRequest => new HttpRequestException("Bad request"),
                    HttpStatusCode.Unauthorized => new UnauthorizedAccessException(),
                    HttpStatusCode.PaymentRequired => new InvalidOperationException("Insufficient credits"),
                    HttpStatusCode.NotFound => new InvalidOperationException("Transcript not found"),
                    HttpStatusCode.RequestTimeout => new HttpRequestException("Temporary error, retry later"),
                    HttpStatusCode.TooManyRequests => new HttpRequestException("Temporary error, retry later"),
                    HttpStatusCode.ServiceUnavailable => new HttpRequestException("Temporary error, retry later"),
                    HttpStatusCode.InternalServerError => new HttpRequestException("Server error"),
                    _ => new HttpRequestException($"Unexpected error: {(int)response.StatusCode}")
                };
            }

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = JsonSerializer.Deserialize<YoutubeTranscriptApiResponse>(json, _jsonOptions)
                ?? throw new InvalidOperationException("Failed to deserialize transcript response");

            return MapToApplicationResponse(result);
        }

        private static GetYoutubeTranscriptResponse MapToApplicationResponse(YoutubeTranscriptApiResponse response)
        {
            return new GetYoutubeTranscriptResponse
            {
                VideoId = response.VideoId,
                Language = response.Language,
                Transcript = response.Transcript.Select(segment => new TranscriptSegmentDto
                {
                    Text = segment.Text,
                    Start = segment.Start,
                    Duration = segment.Duration
                }).ToList(),
                Metadata = response.Metadata is null
                    ? null
                    : new VideoMetadataDto
                    {
                        Title = response.Metadata.Title,
                        AuthorName = response.Metadata.AuthorName,
                        AuthorUrl = response.Metadata.AuthorUrl,
                        ThumbnailUrl = response.Metadata.ThumbnailUrl
                    }
            };
        }
    }
}
