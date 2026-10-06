using DoingTasks.Application.Abstractions.Messaging;
using DoingTasks.SharedKernel.Results;
using FluentValidation;

namespace DoingTasks.Application.Abstractions.Behaviors;

internal static class ValidationDecorator
{
    internal sealed class WithResponse<TCommand, TResponse>(
        ICommandHandler<TCommand, TResponse> inner,
        IEnumerable<IValidator<TCommand>> validators)
        : ICommandHandler<TCommand, TResponse>
        where TCommand : ICommand<TResponse>
    {
        public async Task<Result<TResponse>> Handle(
            TCommand command,
            CancellationToken cancellationToken)
        {
            var errors = await ValidateAsync(command, validators, cancellationToken);

            if (errors.Length > 0)
                return Result.Failure<TResponse>(new ValidationError(errors));

            return await inner.Handle(command, cancellationToken);
        }
    }

    internal sealed class WithoutResponse<TCommand>(
        ICommandHandler<TCommand> inner,
        IEnumerable<IValidator<TCommand>> validators)
        : ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        public async Task<Result> Handle(
            TCommand command,
            CancellationToken cancellationToken)
        {
            var errors = await ValidateAsync(command, validators, cancellationToken);

            if (errors.Length > 0)
                return Result.Failure(new ValidationError(errors));

            return await inner.Handle(command, cancellationToken);
        }
    }

    private static async Task<Error[]> ValidateAsync<TCommand>(
        TCommand command,
        IEnumerable<IValidator<TCommand>> validators,
        CancellationToken cancellationToken)
    {
        if (!validators.Any())
        {
            return [];
        }

        var validationResults = await Task.WhenAll(
                validators.Select(v => v.ValidateAsync(command, cancellationToken)));

        var errors = validationResults
            .SelectMany(r => r.Errors)
            .Where(e => e is not null)
            .Select(e => Error.Validation(e.PropertyName, e.ErrorMessage))
            .ToArray();

        return errors;
    }
}
