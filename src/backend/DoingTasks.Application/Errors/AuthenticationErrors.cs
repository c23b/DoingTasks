using DoingTasks.SharedKernel.Results;

namespace DoingTasks.Application.Errors;

public static class AuthenticationErrors
{
    public static readonly Error InvalidGoogleToken = Error.Problem(
        "Authentication.InvalidGoogleToken", 
        "The Google token is invalid or expired");

    public static readonly Error InvalidMicrosoftToken = Error.Problem(
        "Authentication.InvalidMicrosoftToken", 
        "The Microsoft token is invalid or expired");

    public static readonly Error InvalidCredentials = Error.Problem(
        "Authentication.InvalidCredentials", 
        "Invalid email or password");

    public static readonly Error AccountLockedOut = Error.Problem(
        "Authentication.AccountLockedOut", 
        "Account is locked out due to multiple failed attempts");

    public static readonly Error InvalidRefreshToken = Error.Problem(
        "Authentication.InvalidRefreshToken", 
        "Refresh token is invalid or expired");

    public static readonly Error InvalidEmailConfirmationToken = Error.Problem(
        "Authentication.InvalidEmailConfirmationToken", 
        "Email confirmation token is invalid or expired");

    public static readonly Error InvalidPasswordResetToken = Error.Problem(
        "Authentication.InvalidPasswordResetToken", 
        "Password reset token is invalid or expired");

    public static readonly Error NotFound = Error.NotFound(
        "Authentication.NotFound", 
        "User was not found");

    public static readonly Error IdentityError = Error.Problem(
        "Authentication.IdentityError", 
        "An error occurred while creating the user");
}
