using DoingTasks.Application.Abstractions.Authentication;
using DoingTasks.Application.DTOs.Microsoft;
using DoingTasks.Application.Errors;
using DoingTasks.Infrastructure.ExternalServices.Microsoft;
using DoingTasks.SharedKernel.Results;

namespace DoingTasks.Infrastructure.Authentication.Microsoft;

internal sealed class MicrosoftAuthProvider(IMicrosoftAuthApi microsoftAuthApi) : IMicrosoftAuthProvider
{
    public async Task<Result<MicrosoftUserInfo>> ValidateTokenAsync(
        string accessToken,
        CancellationToken ct = default)
    {
        var response = await microsoftAuthApi.GetUserInfoAsync(accessToken);

        if (!response.IsValid)
            return Result.Failure<MicrosoftUserInfo>(AuthenticationErrors.InvalidMicrosoftToken);

        return Result.Success(new MicrosoftUserInfo
        {
            MicrosoftId = response.Id,
            Email = response.Email,
            Name = response.DisplayName
        });
    }
}