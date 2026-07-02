using DoingTasks.Application.Abstractions.Messaging;
using DoingTasks.SharedKernel.Results;
using Microsoft.Extensions.DependencyInjection;

namespace DoingTasks.Infrastructure.Messaging;

internal sealed class Mediator(IServiceProvider serviceProvider) : IMediator
{
    public async Task<Result> SendAsync<TCommand>(
        TCommand command,
        CancellationToken ct = default)
        where TCommand : ICommand
    {
        var handler = serviceProvider.GetRequiredService<ICommandHandler<TCommand>>();
        return await handler.Handle(command, ct);
    }

    public async Task<Result<TResponse>> SendAsync<TCommand, TResponse>(
        TCommand command,
        CancellationToken ct = default)
        where TCommand : ICommand<TResponse>
    {
        var handler = serviceProvider.GetRequiredService<ICommandHandler<TCommand, TResponse>>();
        return await handler.Handle(command, ct);
    }

    public async Task<Result<TResponse>> QueryAsync<TQuery, TResponse>(
        TQuery query,
        CancellationToken ct = default)
        where TQuery : IQuery<TResponse>
    {
        var handler = serviceProvider.GetRequiredService<IQueryHandler<TQuery, TResponse>>();
        return await handler.Handle(query, ct);
    }
}
