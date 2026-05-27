using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using YoutubeSummarizer.Application.Common.Interfaces;
using YoutubeSummarizer.Application.Features.Summarize;

namespace YoutubeSummarizer.Infrastructure.ExternalServices.Ai
{
    public class AiClient : IAiClient
    {
        private readonly HttpClient _httpClient;
        private readonly AiSettings _settings;
        private readonly ILogger<AiClient> _logger;

        public AiClient(HttpClient httpClient, IOptions<AiSettings> settings, ILogger<AiClient> logger)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
            _logger = logger;
        }

        public async Task<string> CompletePrimaryAsync(string prompt, CancellationToken cancellationToken = default)
        {
            return await SendRequestAsync(
                "https://openrouter.ai/api/v1/chat/completions",
                _settings.ApiKey,
                _settings.Model,
                prompt,
                cancellationToken);
        }

        public async Task<string> CompleteFallbackAsync(string prompt, CancellationToken cancellationToken = default)
        {
            return await SendRequestAsync(
                _settings.FallbackBaseUrl,
                null,
                _settings.FallbackModel,
                prompt,
                cancellationToken);
        }

        private async Task<string> SendRequestAsync(string url, string? apiKey, string model, string prompt, CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            if (!string.IsNullOrEmpty(apiKey))
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            request.Content = JsonContent.Create(new AiRequest
            {
                Model = model,
                Messages = [new AiMessage { Role = "user", Content = prompt }],
                MaxTokens = 4096
            });

            var response = await _httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<AiResponse>(cancellationToken)
                ?? throw new InvalidOperationException("Failed to deserialize chat completion response.");

            return result.Choices?.FirstOrDefault()?.Message?.Content
                ?? throw new InvalidOperationException("Chat completion returned no content.");
        }

        private class AiRequest
        {
            [JsonPropertyName("model")]
            public string Model { get; set; } = string.Empty;

            [JsonPropertyName("messages")]
            public List<AiMessage> Messages { get; set; } = [];

            [JsonPropertyName("max_tokens")]
            public int? MaxTokens { get; set; }
        }

        private class AiMessage
        {
            [JsonPropertyName("role")]
            public string Role { get; set; } = string.Empty;

            [JsonPropertyName("content")]
            public string Content { get; set; } = string.Empty;
        }

        private class AiResponse
        {
            [JsonPropertyName("choices")]
            public List<AiChoice>? Choices { get; set; }
        }

        private class AiChoice
        {
            [JsonPropertyName("message")]
            public AiMessage? Message { get; set; }
        }
    }
}
