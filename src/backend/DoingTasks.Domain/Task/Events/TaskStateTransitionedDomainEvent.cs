using DoingTasks.SharedKernel.Domain;

namespace DoingTasks.Domain;

// Domain/DomainEvents/TaskStateTransitionedDomainEvent.cs
public sealed record TaskStateTransitionedDomainEvent(
    Guid TaskId,
    Guid WorkspaceId,
    Guid FromStateId,
    Guid ToStateId) : IDomainEvent;
