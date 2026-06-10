using DoingTasks.SharedKernel.Results;

namespace DoingTasks.Domain.Tasks;

// Domain/Tasks/StepErrors.cs
public static class StepErrors
{
    public static readonly Error TitleRequired =
        Error.Validation("Step.TitleRequired", "Step title is required");

    public static readonly Error NotFound =
        Error.NotFound("Step.NotFound", "Step was not found");

    public static readonly Error InvalidHours =
        Error.Validation("Step.InvalidHours", "Hours spent cannot be negative");

    public static readonly Error StepAlreadyStatus =
        Error.Validation("Step.StepAlreadyStatus", "The step is already in that status");
}
