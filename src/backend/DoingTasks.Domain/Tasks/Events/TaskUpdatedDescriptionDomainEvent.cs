using DoingTasks.SharedKernel.Domain;

namespace DoingTasks.Domain;

/// <summary>
/// Domain event raised when a worktask had your description updated.
/// </summary>
public sealed record TaskUpdatedDescriptionDomainEvent(
    Guid WorkTaskId,
    string Description) : IDomainEvent;
