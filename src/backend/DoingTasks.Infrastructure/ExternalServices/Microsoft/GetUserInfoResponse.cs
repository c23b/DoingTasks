using System.Text.Json.Serialization;

namespace DoingTasks.Infrastructure.ExternalServices.Microsoft;

public sealed class GetUserInfoResponse
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    [JsonPropertyName("displayName")]
    public string DisplayName { get; init; } = string.Empty;

    [JsonPropertyName("givenName")]
    public string GivenName { get; init; } = string.Empty;

    [JsonPropertyName("surname")]
    public string Surname { get; init; } = string.Empty;

    [JsonPropertyName("mail")]
    public string? Mail { get; init; }

    [JsonPropertyName("userPrincipalName")]
    public string UserPrincipalName { get; init; } = string.Empty;

    [JsonPropertyName("error")]
    public string? Error { get; init; }

    // mail pode ser nulo em contas pessoais — userPrincipalName é o fallback
    public string Email => !string.IsNullOrEmpty(Mail) ? Mail : UserPrincipalName;
    public bool IsValid => string.IsNullOrEmpty(Error) && !string.IsNullOrEmpty(Id);
}
