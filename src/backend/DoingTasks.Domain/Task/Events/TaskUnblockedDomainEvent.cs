using DoingTasks.SharedKernel.Domain;

namespace DoingTasks.Domain;

// Domain/DomainEvents/TaskUnblockedDomainEvent.cs
public sealed record TaskUnblockedDomainEvent(
    Guid TaskId,
    Guid WorkspaceId) : IDomainEvent;
