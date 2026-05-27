using FluentValidation;
using YoutubeSummarizer.Application.Features.UserAccount.Dtos;

namespace YoutubeSummarizer.Application.Features.UserAccount.Validators
{
    public class DeleteAccountRequestValidator : AbstractValidator<DeleteAccountRequest>
    {
        public DeleteAccountRequestValidator()
        {
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.");
        }
    }
}
