using DoingTasks.SharedKernel.Domain;

namespace DoingTasks.Domain;

// Domain/DomainEvents/StepCompletedDomainEvent.cs
public sealed record StepCompletedDomainEvent(
    Guid WorkspaceId,
    Guid TaskId,
    Guid StepId,
    int HoursSpent) : IDomainEvent;