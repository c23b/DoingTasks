using DoingTasks.SharedKernel.Domain;

namespace DoingTasks.Domain;

public sealed record WorkspaceCreatedDomainEvent(
    Guid WorkspaceId,
    Guid OwnerId) : IDomainEvent;
