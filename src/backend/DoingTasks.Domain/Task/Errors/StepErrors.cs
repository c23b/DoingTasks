using DoingTasks.SharedKernel.Results;

namespace DoingTasks.Domain.Task;

// Domain/Tasks/StepErrors.cs
public static class StepErrors
{
    public static readonly Error TitleRequired =
        Error.Validation("Step.TitleRequired", "Step title is required");

    public static readonly Error NotFound =
        Error.NotFound("Step.NotFound", "Step was not found");

    public static readonly Error StateNotMatching =
        Error.Problem("Step.StateNotMatching", "Task must be in the corresponding state to update this step");

    public static readonly Error InvalidHours =
        Error.Validation("Step.InvalidHours", "Hours spent cannot be negative");
}
