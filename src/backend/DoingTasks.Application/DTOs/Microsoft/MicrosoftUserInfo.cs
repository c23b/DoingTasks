using System;
using System.Collections.Generic;
using System.Text;

namespace DoingTasks.Application.DTOs.Microsoft;

public sealed class MicrosoftUserInfo
{
    public string MicrosoftId { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
}
