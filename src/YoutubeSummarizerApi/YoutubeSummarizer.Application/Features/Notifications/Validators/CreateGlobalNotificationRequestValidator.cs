using FluentValidation;
using YoutubeSummarizer.Application.Features.Notifications.Dtos;

namespace YoutubeSummarizer.Application.Features.Notifications.Validators
{
    public class CreateGlobalNotificationRequestValidator : AbstractValidator<CreateGlobalNotificationRequest>
    {
        public CreateGlobalNotificationRequestValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(x => x.Content)
                .NotEmpty()
                .MaximumLength(2000);
        }
    }
}
