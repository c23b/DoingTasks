using DoingTasks.Application.Abstractions.Authentication;
using DoingTasks.Application.Abstractions.Messaging;
using DoingTasks.Application.DTOs.Authentication;
using DoingTasks.SharedKernel.Results;

namespace DoingTasks.Application.Features.Authentication.Commands.LoginByGoogle;

public sealed class LoginByGoogleCommandHandler(
    IGoogleAuthProvider googleAuthProvider,
    IIdentityProvider identityProvider) : ICommandHandler<LoginByGoogleCommand, AuthenticationResponse>
{
    public async Task<Result<AuthenticationResponse>> Handle(
        LoginByGoogleCommand command,
        CancellationToken cancellationToken)
    {
        // 1. Valida o token com o Google
        var googleResult = await googleAuthProvider.AuthenticateAsync(
            command.Code, 
            command.RedirectUri, 
            cancellationToken);

        if (googleResult.IsFailure)
            return Result.Failure<AuthenticationResponse>(googleResult.Error);

        // 2. Busca ApplicationUser pelo login externo do Google
        var loginResult = await identityProvider.GoogleLoginAsync(googleResult.Value.GoogleId);

        if (loginResult.IsFailure)
            return Result.Failure<AuthenticationResponse>(loginResult.Error);

        return Result.Success(loginResult.Value);
    }
}
