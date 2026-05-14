using DoingTasks.SharedKernel.Domain;

namespace DoingTasks.Domain;

// Domain/DomainEvents/MemberInvitedDomainEvent.cs
public sealed record MemberInvitedDomainEvent(
    Guid WorkspaceId,
    Guid UserId) : IDomainEvent;
