using FluentValidation;
using YoutubeSummarizer.Application.Features.Blacklist.Dtos;

namespace YoutubeSummarizer.Application.Features.Blacklist.Validators
{
    public class AddKeywordRequestValidator : AbstractValidator<AddKeywordRequest>
    {
        public AddKeywordRequestValidator()
        {
            RuleFor(x => x.Keyword)
                .NotEmpty().WithMessage("Keyword is required.")
                .MaximumLength(100).WithMessage("Keyword must not exceed 100 characters.");
        }
    }
}