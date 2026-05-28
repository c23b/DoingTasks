using DoingTasks.SharedKernel.Domain;

namespace DoingTasks.Domain;

public sealed record UserCreatedDomainEvent(Guid UserId) : IDomainEvent;
