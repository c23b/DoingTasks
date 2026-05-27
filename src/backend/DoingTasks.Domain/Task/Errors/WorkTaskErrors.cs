using DoingTasks.SharedKernel.Results;

namespace DoingTasks.Domain.Task;

// Domain/Tasks/TaskErrors.cs
public static class WorkTaskErrors
{
    public static readonly Error TitleRequired =
        Error.Validation("Task.TitleRequired", "Title is required");

    public static readonly Error TaskIsBlocked =
        Error.Problem("Task.IsBlocked", "Task is blocked and cannot transition");

    public static readonly Error BlockJustificationRequired =
        Error.Validation("Task.BlockJustificationRequired", "Justification is required to block a task");

    public static readonly Error ActualHoursBelowStepsTotal =
        Error.Validation("Task.ActualHoursBelowStepsTotal", "Actual hours cannot be less than the sum of steps hours");

    public static readonly Error NotFound =
        Error.NotFound("Task.NotFound", "Task was not found");

    public static readonly Error AlreadyBlocked =
        Error.NotFound("Task.AlreadyBlocked", "Task is already blocked");

    public static readonly Error AlreadyUnblocked =
        Error.NotFound("Task.AlreadyUnblocked", "Task is already unblocked");

    public static readonly Error DescriptionRequired =
        Error.Validation("Task.DescriptionRequired", "Description is required");

}
