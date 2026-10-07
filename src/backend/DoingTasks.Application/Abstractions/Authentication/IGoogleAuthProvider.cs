
using DoingTasks.SharedKernel.Results;
using DoingTasks.Application.DTOs.Google;

namespace DoingTasks.Application.Abstractions.Authentication;

public interface IGoogleAuthProvider
{
    Task<Result<GoogleUserInfo>> AuthenticateAsync(
        string code,
        string redirectUri,
        CancellationToken ct = default);
}
