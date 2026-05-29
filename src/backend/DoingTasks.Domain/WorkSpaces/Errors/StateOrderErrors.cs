using DoingTasks.SharedKernel.Results;

namespace DoingTasks.Domain.Workspaces;

// Domain/Workspaces/StateOrderErrors.cs
public static class StateOrderErrors
{
    public static readonly Error Invalid =
        Error.Validation("StateOrder.Invalid", "State order must be greater than 0");
}
