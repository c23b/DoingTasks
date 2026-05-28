using DoingTasks.SharedKernel.Domain;

namespace DoingTasks.Domain;

public sealed record UserUpdatedDomainEvent(Guid UserId) : IDomainEvent;
