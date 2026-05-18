using DoingTasks.SharedKernel.Domain;

namespace DoingTasks.Domain;

public sealed record MemberInvitedDomainEvent(
    Guid WorkspaceId,
    Guid UserId) : IDomainEvent;
