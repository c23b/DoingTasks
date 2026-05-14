using DoingTasks.Domain.WorkSpace;
using DoingTasks.SharedKernel.Domain;
using DoingTasks.SharedKernel.Results;

namespace DoingTasks.Domain.Task;

public sealed class WorkTask : AggregateRoot
{
    public string Title { get; private set; }
    public string? Description { get; private set; }
    public Guid WorkspaceId { get; private set; }
    public Guid CurrentStateId { get; private set; }
    public Guid? GroupId { get; private set; }           // agrupador interno de tasks
    public Complexity Complexity { get; private set; }
    public int? PlannedHours { get; private set; }
    public int ActualHours { get; private set; }
    public bool IsBlocked { get; private set; }
    public string? BlockJustification { get; private set; }
    public Guid? AssignedTo { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private readonly List<Step> _steps = new();
    private readonly List<TaskComment> _comments = new();

    public IReadOnlyCollection<Step> Steps => _steps.AsReadOnly();
    public IReadOnlyCollection<TaskComment> Comments => _comments.AsReadOnly();

    // INVARIANTE: ActualHours nunca inferior à soma dos steps
    public int TotalStepHours => _steps.Sum(s => s.ActualHours);

    private WorkTask() { }

    public static Result<WorkTask> Create(
        string title,
        Guid workspaceId,
        Guid initialStateId,
        int? complexity = null,
        int? plannedHours = null,
        Guid? groupId = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Result.Failure<WorkTask>(WorkTaskErrors.TitleRequired);

        var complexityResult = Complexity.Create(complexity);
        if (complexityResult.IsFailure)
            return Result.Failure<WorkTask>(complexityResult.Error);

        var task = new WorkTask
        {
            Id = Guid.NewGuid(),
            Title = title,
            WorkspaceId = workspaceId,
            CurrentStateId = initialStateId,
            Complexity = complexityResult.Value,
            PlannedHours = plannedHours,
            ActualHours = 0,
            GroupId = groupId,
            CreatedAt = DateTime.UtcNow
        };

        task.RaiseDomainEvent(new TaskCreatedDomainEvent(task.Id, workspaceId, initialStateId));
        return Result.Success(task);
    }

    public Result TransitionTo(Guid newStateId)
    {
        if (IsBlocked)
            return Result.Failure(WorkTaskErrors.TaskIsBlocked);

        var previousStateId = CurrentStateId;
        CurrentStateId = newStateId;

        RaiseDomainEvent(new TaskStateTransitionedDomainEvent(Id, WorkspaceId, previousStateId, newStateId));
        return Result.Success();
    }

    public Result Block(string justification)
    {
        if (string.IsNullOrWhiteSpace(justification))
            return Result.Failure(WorkTaskErrors.BlockJustificationRequired);

        IsBlocked = true;
        BlockJustification = justification;
        RaiseDomainEvent(new TaskBlockedDomainEvent(Id, WorkspaceId, justification));
        return Result.Success();
    }

    public Result Unblock()
    {
        IsBlocked = false;
        BlockJustification = null;
        RaiseDomainEvent(new TaskUnblockedDomainEvent(Id, WorkspaceId));
        return Result.Success();
    }

    public Result UpdateActualHours(int hours)
    {
        // INVARIANTE: horas manuais não podem ser inferiores à soma dos steps
        if (hours < TotalStepHours)
            return Result.Failure(WorkTaskErrors.ActualHoursBelowStepsTotal);

        ActualHours = hours;
        return Result.Success();
    }

    public Result AddStep(string title, Guid workspaceStateId)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Result.Failure(StepErrors.TitleRequired);

        _steps.Add(Step.Create(title, workspaceStateId));
        return Result.Success();
    }

    public Result SetStepDoing(Guid stepId)
    {
        var step = _steps.FirstOrDefault(s => s.Id == stepId);
        if (step is null)
            return Result.Failure(StepErrors.NotFound);

        return step.SetDoing(CurrentStateId);
    }

    public Result SetStepDone(Guid stepId, int hoursSpent)
    {
        var step = _steps.FirstOrDefault(s => s.Id == stepId);
        if (step is null)
            return Result.Failure(StepErrors.NotFound);

        var result = step.SetDone(CurrentStateId, hoursSpent);
        if (result.IsFailure)
            return result;

        // Atualiza horas da task automaticamente após step concluído
        ActualHours = TotalStepHours;

        RaiseDomainEvent(new StepCompletedDomainEvent(Id, stepId, hoursSpent));
        return Result.Success();
    }

    public Result AddComment(string content, Guid authorId)
    {
        var commentResult = TaskComment.Create(content, authorId);
        if (commentResult.IsFailure)
            return Result.Failure(commentResult.Error);

        _comments.Add(commentResult.Value);
        return Result.Success();
    }

    public Result Assign(Guid userId)
    {
        AssignedTo = userId;
        return Result.Success();
    }
}
