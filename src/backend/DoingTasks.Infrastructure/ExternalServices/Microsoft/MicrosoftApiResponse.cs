using System.Text.Json.Serialization;

namespace DoingTasks.Infrastructure.ExternalServices.Microsoft;

public sealed class MicrosoftApiResponse
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    [JsonPropertyName("displayName")]
    public string DisplayName { get; init; } = string.Empty;

    [JsonPropertyName("mail")]
    public string Mail { get; init; } = string.Empty;

    [JsonPropertyName("userPrincipalName")]
    public string UserPrincipalName { get; init; } = string.Empty;

    public string Email => !string.IsNullOrEmpty(Mail) ? Mail : UserPrincipalName;
    public bool IsValid => !string.IsNullOrEmpty(Id) && !string.IsNullOrEmpty(Email);
}