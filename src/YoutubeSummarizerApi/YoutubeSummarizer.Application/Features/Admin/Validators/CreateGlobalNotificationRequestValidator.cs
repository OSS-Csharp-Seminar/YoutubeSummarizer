using FluentValidation;
using YoutubeSummarizer.Application.Features.Admin.Dtos;

namespace YoutubeSummarizer.Application.Features.Admin.Validators
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
