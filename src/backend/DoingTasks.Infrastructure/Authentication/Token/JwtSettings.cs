using System;
using System.Collections.Generic;
using System.Text;

namespace DoingTasks.Infrastructure.Authentication.Token;

public sealed class JwtSettings
{
    public const string SectionName = "JwtSettings";

    public string SecretKey { get; init; } = string.Empty;
    public string Issuer { get; init; } = string.Empty;
    public string Audience { get; init; } = string.Empty;
    public int ExpirationInMinutes { get; init; }
}
