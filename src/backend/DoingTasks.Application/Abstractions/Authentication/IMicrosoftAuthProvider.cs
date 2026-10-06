
using DoingTasks.SharedKernel.Results;
using DoingTasks.Application.DTOs.Microsoft;

namespace DoingTasks.Application.Abstractions.Authentication;

public interface IMicrosoftAuthProvider
{
    Task<Result<MicrosoftUserInfo>> AuthenticateAsync(
        string code,
        string redirectUri,
        string codeVerifier,
        CancellationToken ct = default);
}
