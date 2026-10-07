using Refit;
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace DoingTasks.Infrastructure.ExternalServices.Google;

public sealed class ExchangeCodeResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; init; }

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; init; }

    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; init; }

    [JsonPropertyName("scope")]
    public string Scope { get; init; }

    [JsonPropertyName("token_type")]
    public string TokenType { get; init; }

    [JsonPropertyName("id_token")]
    public string IdToken { get; init; }

    public bool IsValid => !string.IsNullOrEmpty(AccessToken);
}