using DoingTasks.SharedKernel.Domain;

namespace DoingTasks.Domain;

// Domain/DomainEvents/WorkspaceStateAddedDomainEvent.cs
public sealed record WorkspaceStateAddedDomainEvent(
    Guid WorkspaceId,
    Guid StateId) : IDomainEvent;
