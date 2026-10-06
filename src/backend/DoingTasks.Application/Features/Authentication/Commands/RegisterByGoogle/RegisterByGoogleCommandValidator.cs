using FluentValidation;

namespace DoingTasks.Application.Features.Authentication.Commands.RegisterByGoogle;

public sealed class RegisterByGoogleCommandValidator : AbstractValidator<RegisterByGoogleCommand>
{
    public RegisterByGoogleCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty();

        RuleFor(x => x.RedirectUri)
            .NotEmpty();

        RuleFor(x => x.Nickname)
            .NotEmpty()
            .MaximumLength(30);

        RuleFor(x => x.BirthDate)
            .NotEmpty()
            .LessThan(DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-18)))
            .WithMessage("User must be at least 18 years old");
    }
}