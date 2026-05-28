using DoingTasks.SharedKernel.Domain;

namespace DoingTasks.Domain;

/// <summary>
/// Domain event raised when a worktask is retitled.
/// </summary>
public sealed record TaskRetitledDomainEvent(
    Guid WorkTaskId,
    string NewTitle) : IDomainEvent;
