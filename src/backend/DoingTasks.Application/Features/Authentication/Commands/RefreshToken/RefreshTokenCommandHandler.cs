using DoingTasks.Application.Abstractions.Authentication;
using DoingTasks.Application.Abstractions.Messaging;
using DoingTasks.Application.DTOs.Authentication;
using DoingTasks.SharedKernel.Results;

namespace DoingTasks.Application.Features.Authentication.Commands.RefreshToken;

public sealed class RefreshTokenCommandHandler(
    IIdentityProvider identityProvider,
    IUserContext userContext) : ICommandHandler<RefreshTokenCommand, AuthenticationResponse>
{
    public async Task<Result<AuthenticationResponse>> Handle(
        RefreshTokenCommand command,
        CancellationToken cancellationToken)
    {
        var result = await identityProvider.RefreshTokenAsync(userContext.IdentityId);
        if (result.IsFailure)
            return Result.Failure<AuthenticationResponse>(result.Error);

        return Result.Success(result.Value);
    }
}
