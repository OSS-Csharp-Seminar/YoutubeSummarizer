using System.Text;

namespace YoutubeSummarizer.Application.Features.Summarize
{
    public static class PromptBuilder
    {
        public static string Build(string basePrompt, string? additionalInstructions, string transcriptText)
        {
            var sb = new StringBuilder();
            sb.AppendLine(basePrompt);

            if (!string.IsNullOrWhiteSpace(additionalInstructions))
            {
                sb.AppendLine();
                sb.AppendLine("Additional instructions:");
                sb.AppendLine(additionalInstructions);
            }

            sb.AppendLine();
            sb.AppendLine("Transcript:");
            sb.AppendLine(transcriptText);

            return sb.ToString();
        }
    }
}
