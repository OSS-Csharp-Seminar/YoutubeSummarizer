using YoutubeSummarizer.Domain.Enums;

namespace YoutubeSummarizer.Application.Features.AI.PromptTemplates
{
    public class TranscriptPromptTemplateProvider : ITranscriptPromptTemplateProvider
    {
        public string GetTemplate(TranscriptSummarizationStyle style) => style switch
        {
            TranscriptSummarizationStyle.Brief      => BriefPrompt,
            TranscriptSummarizationStyle.Detailed   => DetailedPrompt,
            TranscriptSummarizationStyle.Scientific => ScientificPrompt,
            TranscriptSummarizationStyle.Layman     => LaymanPrompt,
            _ => throw new ArgumentOutOfRangeException(nameof(style))
        };

        private const string BriefPrompt = """
            The user wants a fast, effortless summary of a YouTube video.

            Ignore any timestamps, speaker labels, and promotional segments (ads, sponsorships, merchandise plugs, channel subscribe reminders).

            First, write one sentence (max 25 words) answering: "What is this video about?"
            Then write 3–5 bullets covering only the most important points. Each bullet is one short sentence.

            Tone: casual and clear, like explaining to a friend.
            No jargon. No filler. No opinions.

            If the transcript is too short, incoherent, or mostly non-speech (music, reactions), say:
            "This video doesn't have enough spoken content to summarize reliably."
            """;

        private const string DetailedPrompt = """
            The user wants a thorough, structured summary of a YouTube video with timestamps.
            The transcript includes timestamps in [MM:SS] format — use the nearest one when referencing a moment.

            Organize the summary using this exact structure:

            **Overview** (2–3 sentences): What the video is about and who it's for.

            **Key Points**: The main ideas, arguments, or steps covered.
            Each bullet must open with a timestamp in [MM:SS] format, followed by the point in 1–2 sentences.
            If quoting the speaker directly, wrap the quote in quotation marks after the timestamp.

            Examples:
            - [05:13] "Most men are free only in their dreams" — the speaker uses this to argue that modern society suppresses authentic desire.
            - [17:38] The speaker argues that video games are struggling to retain audiences beyond their core demographic.

            **Takeaways** (2–4 bullets): What the viewer should remember or act on.
            If the video has no clear takeaways (e.g. entertainment, vlog), skip this section.

            Rules:
            - Only use timestamps that appear in the transcript — never invent or estimate them
            - Follow the video's natural order
            - Preserve specific data, names, and numbers when mentioned
            - Ignore sponsor segments, plugs, and off-topic tangents
            - If a section has insufficient content, write "Not applicable" rather than padding it

            If the transcript is too short, incoherent, or mostly non-speech, say:
            "This video doesn't have enough spoken content to summarize reliably."
            """;

        private const string ScientificPrompt = """
            The user wants a rigorous, structured summary of a YouTube video suitable for a scientifically literate audience.
            The transcript includes timestamps in [MM:SS] format — use the nearest one when referencing a specific claim or finding.

            Organize the summary using this exact structure:

            **Content Type**: In one line, identify what kind of video this is.
            Examples: "Expert interview", "Study breakdown", "Opinion/commentary", "Documentary", "Tutorial"

            **Overview** (2–3 sentences): What the video covers, who presents it, and what angle or thesis it takes.

            **Key Claims & Evidence**: The main claims made in the video.
            Each bullet must open with a timestamp in [MM:SS] format.
            For each claim, indicate its support level using one of these tags:
            - [Supported] — backed by data, studies, or direct evidence cited in the video
            - [Anecdotal] — based on personal experience or individual examples
            - [Speculative] — presented as possibility or opinion without evidence
            - [Unsourced] — stated as fact but no source is given

            Example:
            - [08:45] [Supported] A 2021 Stanford study cited by the speaker found that sleep deprivation reduces cognitive performance by up to 40%.
            - [22:10] [Speculative] The speaker suggests that AI will replace most creative jobs within a decade, without citing evidence.

            **Methodology Notes** (if applicable): Note how the speaker arrived at their conclusions —
            personal research, cited studies, expert interviews, anecdotal observation, or opinion.
            If not applicable (e.g. a tutorial or vlog), skip this section.

            **Limitations & Caveats**: Note any gaps, biases, missing counterarguments, or one-sided framing observed in the video.
            If none are apparent, write "None identified."

            **Takeaways**: 2–4 bullets on what is reliably concluded from the video's content.
            Only include what is reasonably supported — do not inflate conclusions beyond what the video presents.

            Rules:
            - Preserve technical terms, study names, and statistics exactly as stated
            - Never editorialize or add outside knowledge — summarize only what is in the transcript
            - Only use timestamps that appear in the transcript — never invent or estimate them
            - Ignore sponsor segments and off-topic tangents
            - If a section has insufficient content, write "Not applicable"

            If the transcript is too short, incoherent, or mostly non-speech, say:
            "This video doesn't have enough spoken content to summarize reliably."
            """;

        private const string LaymanPrompt = """
            The user wants a simple, friendly explanation of a YouTube video —
            as if a knowledgeable friend is catching them up over coffee.

            Organize the summary using this exact structure:

            **What's this about?** (1–2 sentences): The simplest possible answer to "what did I just watch?"
            No jargon. No qualifications. Just the core idea.

            **Here's the gist**: 3–5 bullets explaining the main points in plain everyday language.
            Each bullet should read like something you'd actually say out loud.
            Where the video uses technical terms or complex ideas, replace them with a simple analogy or comparison.

            Example:
            - Instead of "the mitochondria produces ATP through oxidative phosphorylation" write
              "basically your cells have tiny power plants that turn food into usable energy"

            **The bottom line** (1–3 sentences): What should the reader take away or do differently after watching this?
            If the video is purely entertainment or informational with no actionable angle, write what made it interesting or worth watching instead.

            Rules:
            - Never use jargon without immediately explaining it in plain terms
            - No bullet should require re-reading to understand
            - Write in second person where natural ("you", "your") to keep it conversational
            - Keep the whole summary short enough to read in under a minute
            - Ignore timestamps, sponsor segments, and off-topic tangents
            - Follow the video's natural order

            If the transcript is too short, incoherent, or mostly non-speech, say:
            "This video doesn't have enough spoken content to summarize reliably."
            """;
    }
}
