
namespace DoingTasks.Infrastructure.ExternalServices.Common;
public sealed class OAuthErrorResponse
{
    public string Error { get; init; } = string.Empty;
    public string? Error_Description { get; init; }
}