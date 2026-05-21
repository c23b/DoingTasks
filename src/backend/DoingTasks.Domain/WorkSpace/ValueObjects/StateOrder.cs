using DoingTasks.SharedKernel.Results;

namespace DoingTasks.Domain.WorkSpace;

public sealed record StateOrder
{
    public int Value { get; }

    private StateOrder(int value) => Value = value;

    public static Result<StateOrder> Create(int value)
    {
        if (value < 1)
            return Result.Failure<StateOrder>(StateOrderErrors.Invalid);

        return Result.Success(new StateOrder(value));
    }
}
