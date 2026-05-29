using DoingTasks.SharedKernel.Domain;

namespace DoingTasks.Domain;

/// <summary>
/// Domain event raised when workspace states are reordered.
/// </summary>
public sealed record WorkspaceStateReorderedDomainEvent(
    Guid WorkspaceId,
    Guid StateId,
    int NewOrder) : IDomainEvent;
