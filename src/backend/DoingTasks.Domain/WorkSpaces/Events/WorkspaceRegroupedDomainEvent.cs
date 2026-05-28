using DoingTasks.SharedKernel.Domain;

namespace DoingTasks.Domain;

/// <summary>
/// Domain event raised when a workspace group name is changed.
/// </summary>
public sealed record WorkspaceRegroupedDomainEvent(
    Guid WorkspaceId,
    string NewGroupName) : IDomainEvent;
