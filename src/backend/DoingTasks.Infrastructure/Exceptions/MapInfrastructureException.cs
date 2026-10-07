using DoingTasks.Application.Errors;
using DoingTasks.SharedKernel.Results;
using System.Text.Json;

namespace DoingTasks.Infrastructure.Exceptions;

internal static class MapInfrastructureException
{
    internal static Error MapInfrastructureError(Exception ex)
    {
        if (ex is TaskCanceledException)
            return AuthenticationErrors.Timeout;

        if (ex is HttpRequestException)
            return AuthenticationErrors.NetworkError;

        if (ex is JsonException)
            return AuthenticationErrors.InvalidJson;

        return AuthenticationErrors.Unexpected;
    }
}

