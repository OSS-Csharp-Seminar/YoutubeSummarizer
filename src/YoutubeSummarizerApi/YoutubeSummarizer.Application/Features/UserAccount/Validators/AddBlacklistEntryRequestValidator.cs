using FluentValidation;
using YoutubeSummarizer.Application.Features.UserAccount.Dtos;

namespace YoutubeSummarizer.Application.Features.UserAccount.Validators
{
    public class AddBlacklistEntryRequestValidator : AbstractValidator<AddBlacklistEntryRequest>
    {
        public AddBlacklistEntryRequestValidator()
        {
            RuleFor(x => x.Keyword)
                .NotEmpty().WithMessage("Keyword is required.")
                .MaximumLength(100).WithMessage("Keyword must not exceed 100 characters.");
        }
    }
}
