using DoingTasks.SharedKernel.Domain;

namespace DoingTasks.Domain;

public sealed record WorkspaceStateAddedDomainEvent(
    Guid WorkspaceId,
    Guid StateId) : IDomainEvent;
