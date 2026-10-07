using DoingTasks.SharedKernel.Results;

namespace DoingTasks.Application.Errors;

public static class AuthenticationErrors
{
    // ─── Identity ────────────────────────────────────────
    public static readonly Error InvalidCredentials =
        Error.Problem("Authentication.InvalidCredentials", "Invalid email or password");

    public static readonly Error AccountLockedOut =
        Error.Problem("Authentication.AccountLockedOut", "Account is locked out due to multiple failed attempts");

    public static readonly Error InvalidRefreshToken =
        Error.Problem("Authentication.InvalidRefreshToken", "Refresh token is invalid or expired");

    public static readonly Error InvalidEmailConfirmationToken =
        Error.Problem("Authentication.InvalidEmailConfirmationToken", "Email confirmation token is invalid or expired");

    public static readonly Error InvalidPasswordResetToken =
        Error.Problem("Authentication.InvalidPasswordResetToken", "Password reset token is invalid or expired");

    public static readonly Error NotFound =
        Error.NotFound("Authentication.NotFound", "User was not found");

    public static readonly Error IdentityError =
        Error.Problem("Authentication.IdentityError", "An error occurred while creating the user");

    public static readonly Error InvalidToken =
        Error.Problem("Authentication.InvalidToken", "Token is invalid");

    // ─── OAuth Common ─────────────────────────────────────
    public static readonly Error InvalidCode =
        Error.Problem("Authentication.InvalidCode", "The authorization code is invalid");

    public static readonly Error CodeExpired =
        Error.Problem("Authentication.CodeExpired", "The authorization code has expired");

    public static readonly Error CodeAlreadyRedeemed =
        Error.Problem("Authentication.CodeAlreadyRedeemed", "The authorization code has already been used");

    public static readonly Error InvalidRedirectUri =
        Error.Problem("Authentication.InvalidRedirectUri", "The redirect URI does not match");

    public static readonly Error InvalidCodeVerifier =
        Error.Problem("Authentication.InvalidCodeVerifier", "The code verifier does not match the code challenge");

    public static readonly Error InvalidClientSecret =
        Error.Problem("Authentication.InvalidClientSecret", "The client secret is invalid");

    public static readonly Error InvalidAccessToken =
        Error.Problem("Authentication.InvalidAccessToken", "The access token is invalid or expired");

    public static readonly Error InsufficientScope =
        Error.Problem("Authentication.InsufficientScope", "The access token does not have the required scopes");

    public static readonly Error InteractionRequired =
        Error.Problem("Authentication.InteractionRequired", "User interaction is required to complete authentication");

    public static readonly Error ConsentRequired =
        Error.Problem("Authentication.ConsentRequired", "User consent is required");

    public static readonly Error UnauthorizedClient =
        Error.Problem("Authentication.UnauthorizedClient", "The application is not authorized to use this grant type");

    public static readonly Error InvalidScope =
        Error.Problem("Authentication.InvalidScope", "The requested scope is invalid or unknown");

    // ─── Google specific ──────────────────────────────────
    public static readonly Error InvalidGoogleToken = Error.Problem(
        "Authentication.InvalidGoogleToken",
        "The Google token is invalid or expired");

    // ─── Microsoft specific ───────────────────────────────
    public static readonly Error InvalidMicrosoftToken = Error.Problem(
        "Authentication.InvalidMicrosoftToken",
        "The Microsoft token is invalid or expired");

    public static readonly Error MicrosoftTenantNotFound =
        Error.Problem("Authentication.MicrosoftTenantNotFound", "The Microsoft tenant was not found");

    // ─── Infrastructure ───────────────────────────────────
    public static readonly Error ProviderUnavailable =
        Error.Problem("Authentication.ProviderUnavailable", "The authentication provider is unavailable");

    public static readonly Error Timeout =
        Error.Problem("Authentication.Timeout", "The authentication request timed out");

    public static readonly Error NetworkError =
        Error.Problem("Authentication.NetworkError", "A network error occurred while contacting the authentication provider");

    public static readonly Error InvalidJson =
        Error.Problem("Authentication.InvalidJson", "The authentication provider returned an invalid JSON payload");

    public static readonly Error UnknownOAuthError =
        Error.Problem("Authentication.UnknownOAuthError", "An unknown OAuth error occurred");

    public static readonly Error Unexpected =
        Error.Problem("Authentication.Unexpected", "An unexpected error occurred during authentication");

    public static readonly Error AuthenticationFailed =
        Error.Problem("Authentication.AuthenticationFailed", "Authentication Failed");
}
