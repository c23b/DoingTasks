using FluentValidation;

namespace DoingTasks.Application.Features.Authentication.Commands.LoginByMicrosoft;
public sealed class LoginByMicrosoftCommandValidator : AbstractValidator<LoginByMicrosoftCommand>
{
    public LoginByMicrosoftCommandValidator()
    {
        RuleFor(x => x.Code).NotEmpty();
        RuleFor(x => x.RedirectUri).NotEmpty();
        RuleFor(x => x.CodeVerifier).NotEmpty();
    }
}
