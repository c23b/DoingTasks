using Refit;

namespace DoingTasks.Infrastructure.ExternalServices.Microsoft;

public sealed class ExchangeCodeRequest
{
    [AliasAs("client_id")]
    public string ClientId { get; init; } = string.Empty;

    [AliasAs("client_secret")]
    public string ClientSecret { get; init; } = string.Empty;

    [AliasAs("code")]
    public string Code { get; init; } = string.Empty;

    [AliasAs("redirect_uri")]
    public string RedirectUri { get; init; } = string.Empty;

    [AliasAs("grant_type")]
    public string GrantType { get; init; } = "authorization_code";

    [AliasAs("code_verifier")]
    public string CodeVerifier { get; init; } = string.Empty;

    [AliasAs("scope")]
    public string Scope { get; init; } = "openid email profile User.Read";
}