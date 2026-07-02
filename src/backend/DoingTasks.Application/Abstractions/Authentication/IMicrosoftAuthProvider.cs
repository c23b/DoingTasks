
using DoingTasks.SharedKernel.Results;
using DoingTasks.Application.DTOs.Microsoft;

namespace DoingTasks.Application.Abstractions.Authentication;

public interface IMicrosoftAuthProvider
{
    Task<Result<MicrosoftUserInfo>> ValidateTokenAsync(
        string accessToken,
        CancellationToken ct = default);
}
