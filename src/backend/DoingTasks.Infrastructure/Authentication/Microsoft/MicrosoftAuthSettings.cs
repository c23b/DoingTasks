using System;
using System.Collections.Generic;
using System.Text;

namespace DoingTasks.Infrastructure.Authentication.Google;

public sealed class MicrosoftAuthSettings
{
    public const string SectionName = "Authentication:Microsoft";

    public string ClientId { get; init; } = string.Empty;
    public string Secret { get; init; } = string.Empty;
    public string UriToken { get; init; } = string.Empty;
    public string UriUserInfo { get; init; } = string.Empty;
}
