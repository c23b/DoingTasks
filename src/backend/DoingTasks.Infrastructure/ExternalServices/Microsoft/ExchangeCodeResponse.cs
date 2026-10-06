using System.Text.Json.Serialization;

namespace DoingTasks.Infrastructure.ExternalServices.Microsoft;

public sealed class ExchangeCodeResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; init; } = string.Empty;

    [JsonPropertyName("token_type")]
    public string TokenType { get; init; } = string.Empty;

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; init; }

    [JsonPropertyName("error")]
    public string? Error { get; init; }

    [JsonPropertyName("error_description")]
    public string? ErrorDescription { get; init; }

    public bool IsValid => string.IsNullOrEmpty(Error) && !string.IsNullOrEmpty(AccessToken);

}