using FluentValidation;

namespace DoingTasks.Application.Features.Authentication.Commands.LoginByGoogle;
public sealed class LoginByGoogleCommandValidator : AbstractValidator<LoginByGoogleCommand>
{
    public LoginByGoogleCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty();

        RuleFor(x => x.RedirectUri)
            .NotEmpty();
    }
}
