using DoingTasks.Application.Abstractions.Authentication;
using DoingTasks.Application.DTOs.Google;
using DoingTasks.Application.Errors;
using DoingTasks.Infrastructure.ExternalServices.Google;
using DoingTasks.SharedKernel.Results;

namespace DoingTasks.Infrastructure.Authentication.Google;

internal sealed class GoogleAuthProvider(IGoogleAuthApi googleAuthApi) : IGoogleAuthProvider
{
    public async Task<Result<GoogleUserInfo>> ValidateTokenAsync(
        string idToken,
        CancellationToken ct = default)
    {
        var response = await googleAuthApi.ValidateTokenAsync(idToken);

        if (!response.IsValid)
            return Result.Failure<GoogleUserInfo>(AuthenticationErrors.InvalidGoogleToken);

        return Result.Success(new GoogleUserInfo
        {
            GoogleId = response.Sub,
            Email = response.Email,
            Name = response.Name
        });
    }
}
