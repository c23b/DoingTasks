using DoingTasks.SharedKernel.Domain;
using DoingTasks.SharedKernel.Results;

namespace DoingTasks.Domain.Task;

public sealed class Step : Entity
{
    public Guid WorkTaskId { get; private set; }
    public string Title { get; private set; }
    public Guid WorkspaceStateId { get; private set; }
    public bool IsDoing { get; private set; }
    public bool IsDone { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public int ActualHours { get; private set; }
    public Guid? AssignedUserId { get; private set; }

    private Step() { }

    internal static Result<Step> Create(
        string title,
        Guid workTaskId,
        Guid workspaceStateId,
        Guid? assignedUserId = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Result.Failure<Step>(StepErrors.TitleRequired);

        return Result.Success(new Step
        {
            WorkTaskId = workTaskId,
            Title = title,
            WorkspaceStateId = workspaceStateId,
            AssignedUserId = assignedUserId
        });
    }

    // INVARIANTE: Doing só pode ser alterado se a Task estiver no State do Step
    internal Result SetDoing(Guid currentTaskStateId)
    {
        if (currentTaskStateId != WorkspaceStateId)
            return Result.Failure(StepErrors.StateNotMatching);

        IsDoing = true;
        return Result.Success();
    }

    // INVARIANTE: Done só pode ser alterado se a Task estiver no State do Step
    internal Result SetDone(Guid currentTaskStateId, int hoursSpent)
    {
        if (currentTaskStateId != WorkspaceStateId)
            return Result.Failure(StepErrors.StateNotMatching);

        if (hoursSpent < 0)
            return Result.Failure(StepErrors.InvalidHours);

        IsDone = true;
        IsDoing = false;
        ActualHours = hoursSpent;
        CompletedAt = DateTime.UtcNow;
        return Result.Success();
    }

    internal Result Assign(Guid userId)
    {
        AssignedUserId = userId;
        return Result.Success();
    }

    internal Result UpdateWorkspaceState(Guid workspaceStateId)
    {
        WorkspaceStateId = workspaceStateId;
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
