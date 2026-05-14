using DoingTasks.SharedKernel.Results;

namespace DoingTasks.Domain.WorkSpace;

// Domain/Workspaces/StateOrderErrors.cs
public static class StateOrderErrors
{
    public static readonly Error Invalid =
        Error.Validation("StateOrder.Invalid", "State order must be greater than 0");
}
