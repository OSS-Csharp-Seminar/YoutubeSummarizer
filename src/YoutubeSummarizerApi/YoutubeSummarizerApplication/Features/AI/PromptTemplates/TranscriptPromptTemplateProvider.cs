using YoutubeSummarizer.Domain.Enums;

namespace YoutubeSummarizer.Application.Features.AI.PromptTemplates
{
    public class TranscriptPromptTemplateProvider : ITranscriptPromptTemplateProvider
    {
        public string GetTemplate(TranscriptSummarizationStyle style) => style switch
        {
            TranscriptSummarizationStyle.Brief      => "Summarize the transcript in a concise way.",
            TranscriptSummarizationStyle.Detailed   => "Summarize the transcript in detail.",
            TranscriptSummarizationStyle.Scientific => "Summarize the transcript using scientific and technical language.",
            TranscriptSummarizationStyle.Layman     => "Explain the transcript in simple language understandable to everyone.",
            _ => throw new ArgumentOutOfRangeException(nameof(style))
        };
    }
}
