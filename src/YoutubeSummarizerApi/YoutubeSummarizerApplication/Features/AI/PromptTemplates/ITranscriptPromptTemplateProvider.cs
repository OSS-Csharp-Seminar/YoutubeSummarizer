using YoutubeSummarizer.Domain.Enums;

namespace YoutubeSummarizer.Application.Features.AI.PromptTemplates
{
    public interface ITranscriptPromptTemplateProvider
    {
        string GetTemplate(TranscriptSummarizationStyle style);
    }
}
