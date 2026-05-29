using DoingTasks.SharedKernel.Domain;

namespace DoingTasks.Domain;

/// <summary>
/// Domain event raised when a workspace state is renamed.
/// </summary>
public sealed record WorkspaceStateRenamedDomainEvent(
    Guid WorkspaceId,
    Guid StateId,
    string NewName) : IDomainEvent;
