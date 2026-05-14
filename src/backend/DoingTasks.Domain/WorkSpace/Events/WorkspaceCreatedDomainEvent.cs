using DoingTasks.SharedKernel.Domain;

namespace DoingTasks.Domain;

// Domain/DomainEvents/WorkspaceCreatedDomainEvent.cs
public sealed record WorkspaceCreatedDomainEvent(
    Guid WorkspaceId,
    Guid OwnerId) : IDomainEvent;
