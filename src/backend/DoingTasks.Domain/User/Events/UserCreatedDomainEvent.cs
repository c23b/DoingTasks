using DoingTasks.SharedKernel.Domain;

namespace DoingTasks.Domain;

// Domain/DomainEvents/UserCreatedDomainEvent.cs
public sealed record UserCreatedDomainEvent(Guid UserId) : IDomainEvent;
