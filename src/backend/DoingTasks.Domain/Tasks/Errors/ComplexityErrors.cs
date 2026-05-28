using DoingTasks.SharedKernel.Results;

namespace DoingTasks.Domain.Tasks;

// Domain/Tasks/ComplexityErrors.cs
public static class ComplexityErrors
{
    public static readonly Error OutOfRange =
        Error.Validation("Complexity.OutOfRange", "Complexity must be between 1 and 10");
}
