
using DoingTasks.Application.DTOs.Authentication;
using DoingTasks.SharedKernel.Results;

namespace DoingTasks.Application.Abstractions.Authentication;

public interface IIdentityProvider
{
    Task<Result<RegisterResult>> RegisterAsync(string email, string password, Guid domainUserId);
    Task<Result<AuthenticationResponse>> LoginAsync(string email, string password);
    Task<Result<AuthenticationResponse>> RefreshTokenAsync(string identityId);
    Task<Result> ConfirmEmailAsync(string userId, string token);
    Task<Result> ResendConfirmationEmailAsync(string email);
    Task<Result> ForgotPasswordAsync(string email);
    Task<Result> ResetPasswordAsync(string email, string token, string newPassword);
    Task<Result> DeleteAsync(string identityId);
    Task<Result<RegisterResult>> MicrosoftRegisterAsync(string microsoftId,string email, string name, Guid domainUserId);
    Task<Result<RegisterResult>> GoogleRegisterAsync(string googleId,string email,string name, Guid domainUserId);
    Task<Result<AuthenticationResponse>> GoogleLoginAsync(string googleId);
    Task<Result<AuthenticationResponse>> MicrosoftLoginAsync(string microsoftId);
}
