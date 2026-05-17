using Microsoft.Extensions.Options;
using YoutubeSummarizer.Application.Common.Interfaces;
using YoutubeSummarizer.Application.Common.Models;
using YoutubeSummarizer.Application.Features.Summarize.Dtos;
using YoutubeSummarizer.Application.Features.Summarize.Interfaces;
using YoutubeSummarizer.Application.Features.YoutubeTranscript.Dtos;
using YoutubeSummarizer.Application.Features.YoutubeTranscript.Interfaces;

namespace YoutubeSummarizer.Application.Features.Summarize.Services
{
    public class SummarizeService : ISummarizeService
    {
        private readonly IYoutubeTranscriptService _transcriptService;
        private readonly IAiClient _aiClient;
        private readonly AiSettings _aiSettings;

        public SummarizeService(
            IYoutubeTranscriptService transcriptService,
            IAiClient aiClient,
            IOptions<AiSettings> aiSettings)
        {
            _transcriptService = transcriptService;
            _aiClient = aiClient;
            _aiSettings = aiSettings.Value;
        }

        public async Task<ServiceResponse<SummarizeResponse>> SummarizeAsync(SummarizeRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var transcriptResponse = await _transcriptService.GetTranscriptAsync(
                    new GetYoutubeTranscriptRequest { VideoUrl = request.VideoUrl },
                    cancellationToken);

                if (!transcriptResponse.Status)
                    return ServiceResponse<SummarizeResponse>.Failure(transcriptResponse.Message);

                var transcriptText = string.Join("\n", transcriptResponse.Data!.Transcript.Select(s => s.Text));

                var prompt = PromptBuilder.Build(_aiSettings.BasePrompt, request.AdditionalInstructions, transcriptText);

                var content = await _aiClient.CompleteAsync(prompt, cancellationToken);

                return ServiceResponse<SummarizeResponse>.Success(
                    new SummarizeResponse { Content = content },
                    "Sažetak uspješno generiran.");
            }
            catch
            {
                return ServiceResponse<SummarizeResponse>.Failure("Došlo je do greške pri generiranju sažetka.");
            }
        }
    }
}
