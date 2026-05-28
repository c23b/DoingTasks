using DoingTasks.SharedKernel.Domain;

namespace DoingTasks.Domain;

// Domain/DomainEvents/TaskBlockedDomainEvent.cs
public sealed record TaskBlockedDomainEvent(
    Guid TaskId,
    Guid WorkspaceId,
    string Justification) : IDomainEvent;
