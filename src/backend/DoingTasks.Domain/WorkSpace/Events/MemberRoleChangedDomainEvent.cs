using DoingTasks.SharedKernel.Domain;
using DoingTasks.Domain.WorkSpace;

namespace DoingTasks.Domain;

/// <summary>
/// Domain event raised when a workspace member's role is changed.
/// </summary>
public sealed record MemberRoleChangedDomainEvent(
    Guid WorkspaceId,
    Guid UserId,
    MemberRole NewRole) : IDomainEvent;
