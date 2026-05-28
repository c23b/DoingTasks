using DoingTasks.SharedKernel.Domain;

namespace DoingTasks.Domain;

// Domain/DomainEvents/TaskCreatedDomainEvent.cs
public sealed record TaskCreatedDomainEvent(
    Guid TaskId,
    Guid WorkspaceId,
    Guid InitialStateId) : IDomainEvent;
