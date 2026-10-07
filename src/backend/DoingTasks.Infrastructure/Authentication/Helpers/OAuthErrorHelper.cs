using DoingTasks.Application.Errors;
using DoingTasks.Infrastructure.ExternalServices.Common;
using DoingTasks.SharedKernel.Results;

namespace DoingTasks.Infrastructure.Authentication.Helpers;

public static class OAuthErrorHelper
{
    public static Error Classify(string endpoint, OAuthErrorResponse? error)
    {
        if (error is null)
            return AuthenticationErrors.UnknownOAuthError;

        var e = error.Error ?? string.Empty;
        var d = error.Error_Description ?? string.Empty;

        return endpoint switch
        {
            "token" => ClassifyTokenError(e, d),
            "userinfo" => ClassifyUserInfoError(e, d),
            _ => AuthenticationErrors.UnknownOAuthError
        };
    }

    private static Error ClassifyTokenError(string error, string description)
    {
        // Código já usado — Microsoft: AADSTS70002/AADSTS54005
        if (description.Contains("already redeemed") || description.Contains("AADSTS54005"))
            return HundleInternalErrors(AuthenticationErrors.CodeAlreadyRedeemed);

        return error switch
        {
            // invalid_grant
            "invalid_grant" when description.Contains("verifier")
                => AuthenticationErrors.InvalidCodeVerifier,

            "invalid_grant" when description.Contains("expired") || description.Contains("AADSTS70008")
                => AuthenticationErrors.CodeExpired,

            "invalid_grant" when description.Contains("redeemed") || description.Contains("AADSTS54005")
                => HundleInternalErrors(AuthenticationErrors.CodeAlreadyRedeemed),

            "invalid_grant"
                => AuthenticationErrors.InvalidCode,

            //invalid_request
            "invalid_request" when description.Contains("redirect_uri") || description.Contains("redirect")
                => AuthenticationErrors.InvalidRedirectUri,

            // PKCE obrigatório mas ausente
            "invalid_request" when description.Contains("code_verifier") || description.Contains("PKCE") || description.Contains("code_challenge")
                => AuthenticationErrors.InvalidCodeVerifier,

            "invalid_request" when description.Contains("AADSTS90023") // public client secret
                => HundleInternalErrors(AuthenticationErrors.InvalidClientSecret),

            "invalid_request"
                => AuthenticationErrors.InvalidCode,

            //redirect_uri_mismatch
            "redirect_uri_mismatch"
                => AuthenticationErrors.InvalidRedirectUri,

            //unauthorized_client
            "unauthorized_client"
                => HundleInternalErrors(AuthenticationErrors.UnauthorizedClient),

            //invalid_client
            "invalid_client"
                => HundleInternalErrors(AuthenticationErrors.InvalidClientSecret),

            //invalid_scope
            "invalid_scope"
                => HundleInternalErrors(AuthenticationErrors.InvalidScope),

            // ─── admin_policy_enforced (Google) ───────────────────────────
            "admin_policy_enforced"
                => HundleInternalErrors(AuthenticationErrors.InsufficientScope),

            // ─── deleted_client (Google) ───────────────────────
            "deleted_client"
                => AuthenticationErrors.InvalidCode,

            //interaction_required (Microsoft)
            "interaction_required"
                => AuthenticationErrors.InteractionRequired,

            //consent_required (Microsoft)
            "consent_required"
                => AuthenticationErrors.ConsentRequired,

            //tenant not found (Microsoft) ─────────────
            _ when description.Contains("AADSTS90002") || description.Contains("tenant")
                => HundleInternalErrors(AuthenticationErrors.MicrosoftTenantNotFound),

            //server errors
            "server_error" or "temporarily_unavailable"
                => AuthenticationErrors.ProviderUnavailable,

            _ => HundleInternalErrors(AuthenticationErrors.UnknownOAuthError)
        };
    }

    private static Error ClassifyUserInfoError(string error, string description)
    {
        return error switch
        {
            // ─── invalid_token ────────────────────────────
            "invalid_token" or "unauthorized"
                => AuthenticationErrors.InvalidAccessToken,

            // ─── insufficient_scope ───────────────────────
            "insufficient_scope" or "insufficient_claims"
                => AuthenticationErrors.InsufficientScope,

            // ─── Google specific ──────────────────────────
            _ when description.Contains("expired")
                => AuthenticationErrors.InvalidAccessToken,

            _ => AuthenticationErrors.UnknownOAuthError
        };
    }

    private static Error HundleInternalErrors(Error error) 
    {
        //TODO: Implemment Logging for internal errors
        return AuthenticationErrors.AuthenticationFailed;
    }
}


