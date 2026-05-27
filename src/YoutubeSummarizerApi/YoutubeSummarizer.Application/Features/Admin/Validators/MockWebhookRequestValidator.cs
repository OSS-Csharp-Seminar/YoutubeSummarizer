using FluentValidation;
using YoutubeSummarizer.Application.Features.Admin.Dtos;

namespace YoutubeSummarizer.Application.Features.Admin.Validators
{
    public class MockWebhookRequestValidator : AbstractValidator<MockWebhookRequest>
    {
        public MockWebhookRequestValidator()
        {
            RuleFor(x => x.VideoUrl)
                .NotEmpty()
                .Must(url => url.Contains("youtube.com/watch") || url.Contains("youtu.be/"));
        }
    }
}
