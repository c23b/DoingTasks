using DoingTasks.SharedKernel.Services;

namespace DoingTasks.Infrastructure.Time;

internal sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}