using DoingTasks.SharedKernel.Results;

namespace DoingTasks.Application.Abstractions.Messaging;

public interface IMediator
{
    Task<Result> SendAsync<TCommand>(
        TCommand command,
        CancellationToken ct = default)
        where TCommand : ICommand;

    Task<Result<TResponse>> SendAsync<TCommand, TResponse>(
        TCommand command,
        CancellationToken ct = default)
        where TCommand : ICommand<TResponse>;

    Task<Result<TResponse>> QueryAsync<TQuery, TResponse>(
        TQuery query,
        CancellationToken ct = default)
        where TQuery : IQuery<TResponse>;
}