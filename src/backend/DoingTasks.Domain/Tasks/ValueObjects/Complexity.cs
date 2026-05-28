using DoingTasks.SharedKernel.Results;

namespace DoingTasks.Domain.Tasks;

public sealed record Complexity
{
    public int? Value { get; }

    private Complexity(int? value) => Value = value;

    public static readonly Complexity Undefined = new((int?)null);

    public static Result<Complexity> Create(int? value)
    {
        if (value is not null && (value < 1 || value > 10))
            return Result.Failure<Complexity>(ComplexityErrors.OutOfRange);

        return Result.Success(new Complexity(value));
    }
}
