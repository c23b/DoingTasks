
using DoingTasks.SharedKernel.Results;

namespace DoingTasks.Application.Abstractions.Authentication;

public interface IIdentityProvider
{
    Task<Result<string>> RegisterAsync(string email, string password, Guid domainUserId);
    Task<Result<string>> LoginAsync(string email, string password);
    Task<Result<string>> RefreshTokenAsync(string userId, string refreshToken);
    Task<Result> ConfirmEmailAsync(string userId, string token);
    Task<Result> ResendConfirmationEmailAsync(string email);
    Task<Result> ForgotPasswordAsync(string email);
    Task<Result> ResetPasswordAsync(string email, string token, string newPassword);
}
