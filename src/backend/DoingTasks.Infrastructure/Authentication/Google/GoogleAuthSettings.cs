using System;
using System.Collections.Generic;
using System.Text;

namespace DoingTasks.Infrastructure.Authentication.Google;

public sealed class GoogleAuthSettings
{
    public const string SectionName = "Authentication:Google";

    public string ClientId { get; init; } = string.Empty;
    public string Secret { get; init; } = string.Empty;
    public string UriToken { get; init; } = string.Empty;
    public string UriUserInfo { get; init; } = string.Empty;
}
