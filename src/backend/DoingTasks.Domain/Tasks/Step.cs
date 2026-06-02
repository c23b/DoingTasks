using DoingTasks.SharedKernel.Domain;
using DoingTasks.SharedKernel.Results;

namespace DoingTasks.Domain.Tasks;

public sealed class Step : Entity
{
    public Guid WorkTaskId { get; private set; }
    public string Title { get; private set; }
    public StepStatus StepStatus { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public int ActualHours { get; private set; }
    public Guid? AssignedUserId { get; private set; }

    private Step() { }

    internal static Result<Step> Create(
        string title,
        Guid workTaskId,
        Guid? assignedUserId = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Result.Failure<Step>(StepErrors.TitleRequired);

        return Result.Success(new Step
        {
            WorkTaskId = workTaskId,
            Title = title,
            AssignedUserId = assignedUserId,
            StepStatus = StepStatus.Pending
        });
    }

    internal Result SetStepStatusPending()
    {
        if(StepStatus == StepStatus.Pending)
            return Result.Failure(StepErrors.StepAlreadyStatus);

        StepStatus = StepStatus.Pending;
        return Result.Success();
    }

    internal Result SetStepStatusDoing()
    {
        if (StepStatus == StepStatus.Doing)
            return Result.Failure(StepErrors.StepAlreadyStatus);

        StepStatus = StepStatus.Doing;
        return Result.Success();
    }

    internal Result SetStepStatusDone(int hoursSpent, DateTime completedAt)
    {
       if (hoursSpent < 0)
            return Result.Failure(StepErrors.InvalidHours);

        if (StepStatus == StepStatus.Done)
            return Result.Failure(StepErrors.StepAlreadyStatus);

        StepStatus = StepStatus.Done;
        ActualHours = hoursSpent;
        CompletedAt = completedAt;
        return Result.Success();
    }

    internal Result Assign(Guid userId)
    {
        AssignedUserId = userId;
        return Result.Success();
    }      

    internal Result Retitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Result.Failure<Step>(StepErrors.TitleRequired);

        Title = title;
        return Result.Success();
    }
}
