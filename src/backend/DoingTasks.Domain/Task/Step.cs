using DoingTasks.SharedKernel.Domain;
using DoingTasks.SharedKernel.Results;

namespace DoingTasks.Domain.Task;

// ─── TASK ────────────────────────────────────────────────

public sealed class Step : Entity
{
    public string Title { get; private set; }
    public Guid WorkspaceStateId { get; private set; }
    public bool IsDoing { get; private set; }
    public bool IsDone { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public int ActualHours { get; private set; }
    public Guid? AssignedTo { get; private set; }

    private Step() { }

    internal static Step Create(string title, Guid workspaceStateId) =>
        new()
        {
            Id = Guid.NewGuid(),
            Title = title,
            WorkspaceStateId = workspaceStateId
        };

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
        AssignedTo = userId;
        return Result.Success();
    }
}
