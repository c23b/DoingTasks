using DoingTasks.Application.Abstractions.Authentication;
using DoingTasks.Application.Abstractions.Messaging;
using DoingTasks.Application.DTOs.Authentication;
using DoingTasks.SharedKernel.Results;

namespace DoingTasks.Application.Features.Authentication.Commands.Login;

public sealed class LoginCommandHandler(IIdentityProvider identityProvider) : ICommandHandler<LoginCommand, AuthenticationResponse>
{
    public async Task<Result<AuthenticationResponse>> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        var result = await identityProvider.LoginAsync(command.Email, command.Password);
        if (result.IsFailure)
            return Result.Failure<AuthenticationResponse>(result.Error);

        return Result.Success(result.Value);
    }
}
