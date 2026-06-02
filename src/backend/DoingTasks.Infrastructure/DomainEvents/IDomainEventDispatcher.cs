using DoingTasks.SharedKernel.Domain;

namespace DoingTasks.Infrastructure.DomainEvents;

public interface IDomainEventDispatcher
{
    Task DispatchAsync(
        IReadOnlyList<IDomainEvent> domainEvents,
        CancellationToken cancellationToken = default);
}
