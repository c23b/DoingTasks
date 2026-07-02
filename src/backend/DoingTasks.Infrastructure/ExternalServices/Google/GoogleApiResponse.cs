using System.Text.Json.Serialization;

namespace DoingTasks.Infrastructure.ExternalServices.Google;

public sealed class GoogleApiResponse
{
    [JsonPropertyName("sub")]
    public string Sub { get; init; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; init; } = string.Empty;

    [JsonPropertyName("email_verified")]
    public string EmailVerified { get; init; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("picture")]
    public string Picture { get; init; } = string.Empty;

    [JsonPropertyName("error_description")]
    public string? ErrorDescription { get; init; }

    public bool IsValid => string.IsNullOrEmpty(ErrorDescription)
                        && EmailVerified == "true";
}
