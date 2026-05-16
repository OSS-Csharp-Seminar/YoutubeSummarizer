using FluentValidation;

namespace YoutubeSummarizer.Application.Features.YoutubeChannels.SubscribeToYoutubeChannel
{
    public class SubscribeToYoutubeChannelRequestValidator : AbstractValidator<SubscribeToYoutubeChannelRequest>
    {
        public SubscribeToYoutubeChannelRequestValidator()
        {
            RuleFor(x => x.ChannelUrl)
                .NotEmpty()
                .Matches(@"^https?://(www\.)?youtube\.com/(@[\w.-]+|channel/[\w-]+)")
                .WithMessage("Must be a valid YouTube channel URL.");

            RuleFor(x => x.SummarizationStyle)
                .IsInEnum();
        }
    }
}
