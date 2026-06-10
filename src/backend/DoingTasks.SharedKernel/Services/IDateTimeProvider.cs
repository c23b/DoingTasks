namespace DoingTasks.SharedKernel.Services;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}