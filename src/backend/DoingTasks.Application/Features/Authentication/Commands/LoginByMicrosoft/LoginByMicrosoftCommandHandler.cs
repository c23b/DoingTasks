using DoingTasks.Application.Abstractions.Authentication;
using DoingTasks.Application.Abstractions.Messaging;
using DoingTasks.Application.DTOs.Authentication;
using DoingTasks.SharedKernel.Results;

namespace DoingTasks.Application.Features.Authentication.Commands.LoginByMicrosoft;

public sealed class LoginByMicrosoftCommandHandler(
    IMicrosoftAuthProvider microsoftAuthProvider,
    IIdentityProvider identityProvider) : ICommandHandler<LoginByMicrosoftCommand, AuthenticationResponse>
{
    public async Task<Result<AuthenticationResponse>> Handle(
        LoginByMicrosoftCommand command,
        CancellationToken cancellationToken)
    {
        // 1. Valida o token com a Microsoft
        var microsoftResult = await microsoftAuthProvider.AuthenticateAsync(
            command.Code,
            command.RedirectUri,
            command.CodeVerifier,
            cancellationToken);

        if (microsoftResult.IsFailure)
            return Result.Failure<AuthenticationResponse>(microsoftResult.Error);

        // 2. Busca ApplicationUser pelo login externo da Microsoft
        var loginResult = await identityProvider.MicrosoftLoginAsync(
            microsoftResult.Value.MicrosoftId);

        if (loginResult.IsFailure)
            return Result.Failure<AuthenticationResponse>(loginResult.Error);

        return Result.Success(loginResult.Value);
    }
}
