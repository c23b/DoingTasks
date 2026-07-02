using System;
using System.Collections.Generic;
using System.Text;

namespace DoingTasks.Application.DTOs.Google;

public sealed class GoogleUserInfo
{
    public string GoogleId { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
}
