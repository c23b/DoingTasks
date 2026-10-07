using Refit;

namespace DoingTasks.Infrastructure.ExternalServices.Google;

public sealed class ExchangeCodeRequest
{
    [AliasAs("code")]
    public string Code { get; set; }

    [AliasAs("client_id")]
    public string ClientId { get; set; }

    [AliasAs("client_secret")]
    public string ClientSecret { get; set; }

    [AliasAs("redirect_uri")]
    public string RedirectUri { get; set; }

    [AliasAs("grant_type")]
    public string GrantType { get; set; } = "authorization_code";

    [AliasAs("code_verifier")]
    public string CodeVerifier { get; set; }
}