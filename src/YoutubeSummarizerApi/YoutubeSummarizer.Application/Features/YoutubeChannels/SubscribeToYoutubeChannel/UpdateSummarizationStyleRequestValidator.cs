using FluentValidation;

namespace YoutubeSummarizer.Application.Features.YoutubeChannels.SubscribeToYoutubeChannel
{
    public class UpdateSummarizationStyleRequestValidator : AbstractValidator<UpdateSummarizationStyleRequest>
    {
        public UpdateSummarizationStyleRequestValidator()
        {
            RuleFor(x => x.SummarizationStyle)
                .IsInEnum();
        }
    }
}
