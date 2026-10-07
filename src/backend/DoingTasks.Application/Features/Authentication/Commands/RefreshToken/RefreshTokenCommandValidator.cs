using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace DoingTasks.Application.Features.Authentication.Commands.RefreshToken;
public sealed class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        
    }
}
