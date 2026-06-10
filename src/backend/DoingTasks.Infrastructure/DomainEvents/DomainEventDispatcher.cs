using DoingTasks.SharedKernel.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace DoingTasks.Infrastructure.DomainEvents;

internal sealed class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public DomainEventDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task DispatchAsync(
        IReadOnlyList<IDomainEvent> domainEvents,
        CancellationToken cancellationToken = default)
    {
        foreach (var domainEvent in domainEvents)
            await DispatchEventAsync(domainEvent, cancellationToken);
    }

    private async Task DispatchEventAsync(
        IDomainEvent domainEvent,
        CancellationToken cancellationToken)
    {
        var handlerType = typeof(IDomainEventHandler<>)
            .MakeGenericType(domainEvent.GetType());

        var handlers = _serviceProvider.GetServices(handlerType);

        foreach (var handler in handlers)
        {
            await (Task)handlerType
                .GetMethod(nameof(IDomainEventHandler<IDomainEvent>.HandleAsync))!
                .Invoke(handler, [domainEvent, cancellationToken])!;
        }
    }
}
