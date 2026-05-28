using DoingTasks.SharedKernel.Domain;

namespace DoingTasks.Domain;

/// <summary>
/// Domain event raised when a workspace is renamed.
/// </summary>
public sealed record WorkspaceRenamedDomainEvent(
    Guid WorkspaceId,
    string NewName) : IDomainEvent;
