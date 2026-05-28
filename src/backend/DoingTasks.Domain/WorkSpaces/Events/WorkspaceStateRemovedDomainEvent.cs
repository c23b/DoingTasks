using DoingTasks.SharedKernel.Domain;

namespace DoingTasks.Domain;

public sealed record WorkspaceStateRemovedDomainEvent(
    Guid WorkspaceId,
    Guid StateId) : IDomainEvent;
