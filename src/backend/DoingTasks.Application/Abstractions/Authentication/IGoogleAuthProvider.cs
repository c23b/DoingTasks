
using DoingTasks.SharedKernel.Results;
using DoingTasks.Application.DTOs.Google;

namespace DoingTasks.Application.Abstractions.Authentication;

public interface IGoogleAuthProvider
{
    Task<Result<GoogleUserInfo>> ValidateTokenAsync(
        string idToken,
        CancellationToken ct = default);
}
