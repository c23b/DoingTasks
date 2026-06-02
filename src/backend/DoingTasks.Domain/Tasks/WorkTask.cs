using DoingTasks.Domain.Workspaces;
using DoingTasks.SharedKernel.Domain;
using DoingTasks.SharedKernel.Results;

namespace DoingTasks.Domain.Tasks;

public sealed class WorkTask : AggregateRoot
{
    public string Title { get; private set; }
    public string? Description { get; private set; }
    public Guid WorkspaceId { get; private set; }
    public Guid CurrentStateId { get; private set; }
    public Complexity Complexity { get; private set; }
    public int? PlannedHours { get; private set; }
    public int ActualHours { get; private set; }
    public bool IsBlocked { get; private set; }
    public string? BlockJustification { get; private set; }
    public Guid? AssignedUserId { get; private set; }

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
        string? description = null,
        int? plannedHours = null,
        Guid? assignedUserId = null,
        int? complexity = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Result.Failure<WorkTask>(WorkTaskErrors.TitleRequired);

        var complexityResult = Complexity.Create(complexity);
        if (complexityResult.IsFailure)
            return Result.Failure<WorkTask>(complexityResult.Error);

        var task = new WorkTask
        {
            Title = title,
            WorkspaceId = workspaceId,
            CurrentStateId = initialStateId,
            Description = description,
            Complexity = complexityResult.Value,
            PlannedHours = plannedHours,
            ActualHours = 0,
            AssignedUserId = assignedUserId
        };

        task.RaiseDomainEvent(new TaskCreatedDomainEvent(task.Id, workspaceId, initialStateId));
        return Result.Success(task);
    }

    public Result Retitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Result.Failure<Workspace>(WorkTaskErrors.TitleRequired);

        Title = title;
        RaiseDomainEvent(new TaskRetitledDomainEvent(Id, title));

        return Result.Success();
    }

    public Result UpdateDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            return Result.Failure<Workspace>(WorkTaskErrors.DescriptionRequired);

        Description = description;
        RaiseDomainEvent(new TaskRetitledDomainEvent(Id, description));

        return Result.Success();
    }

    public Result TransitionToState(Guid newStateId)
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
        if (IsBlocked is true)
            return Result.Failure(WorkTaskErrors.AlreadyBlocked);

        if (string.IsNullOrWhiteSpace(justification))
            return Result.Failure(WorkTaskErrors.BlockJustificationRequired);

        IsBlocked = true;
        BlockJustification = justification;
        RaiseDomainEvent(new TaskBlockedDomainEvent(Id, WorkspaceId, justification));
        return Result.Success();
    }

    public Result Unblock()
    {
        if (IsBlocked is false)
            return Result.Failure(WorkTaskErrors.AlreadyUnblocked);

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

    public Result UpdateComplexity(int? complexity)
    {
        var complexityResult = Complexity.Create(complexity);

        if (complexityResult.IsFailure)
            return Result.Failure(complexityResult.Error);

        Complexity = complexityResult.Value;

        return Result.Success();
    }

    public Result UpdatePlannedHours(int? hours)
    {
        PlannedHours = hours;
        return Result.Success();
    }

    public Result Assign(Guid userId)
    {
        AssignedUserId = userId;
        return Result.Success();
    }

    public Result AddStep(string title, Guid? assignedUserId = null)
    {
        var stepResult = Step.Create(title, this.Id, assignedUserId);
        if (stepResult.IsFailure)
            return Result.Failure(stepResult.Error);

        _steps.Add(stepResult.Value);
        return Result.Success();
    }

    public Result RemoveStep(Guid stepId)
    {
        var step = _steps.FirstOrDefault(s => s.Id == stepId);
        
        if (step is null)
            return Result.Failure(StepErrors.NotFound);

        _steps.Remove(step);
        return Result.Success();
    }

    public Result SetStepStatusPending(Guid stepId)
    {
        var step = _steps.FirstOrDefault(s => s.Id == stepId);
        if (step is null)
            return Result.Failure(StepErrors.NotFound);

        return step.SetStepStatusPending();
    }

    public Result SetStepStatusDoing(Guid stepId)
    {
        var step = _steps.FirstOrDefault(s => s.Id == stepId);
        if (step is null)
            return Result.Failure(StepErrors.NotFound);

        return step.SetStepStatusDoing();
    }

    public Result SetStepStatusDone(Guid stepId, int hoursSpent, DateTime completedAt)
    {
        var step = _steps.FirstOrDefault(s => s.Id == stepId);
        if (step is null)
            return Result.Failure(StepErrors.NotFound);

        var result = step.SetStepStatusDone(hoursSpent, completedAt);
        if (result.IsFailure)
            return result;

        // Atualiza horas da task automaticamente após step concluído
        ActualHours = TotalStepHours;

        RaiseDomainEvent(new StepCompletedDomainEvent(WorkspaceId, Id, stepId, hoursSpent));
        return Result.Success();
    }

    public Result AssignStep(Guid stepId, Guid userId)
    {
        var step = _steps.FirstOrDefault(s => s.Id == stepId);
        if (step is null)
            return Result.Failure(StepErrors.NotFound);
                
        return step.Assign(userId);
    }

    public Result RetitleStep(Guid stepId, string title)
    {
        var step = _steps.FirstOrDefault(s => s.Id == stepId);
        if (step is null)
            return Result.Failure(StepErrors.NotFound);

        return step.Retitle(title);
    }

    public Result AddComment(string content, Guid authorId)
    {
        var commentResult = TaskComment.Create(this.Id, authorId, content);
        if (commentResult.IsFailure)
            return Result.Failure(commentResult.Error);

        _comments.Add(commentResult.Value);
        return Result.Success();
    }

    public Result UpdateComment(Guid taskCommentId, string content)
    {
        var comment = _comments.FirstOrDefault(s => s.Id == taskCommentId);
        if (comment is null)
            return Result.Failure(TaskCommentErrors.NotFound);

        return comment.Update(content);
    }

    public Result RemoveComment(Guid taskCommentId, string content)
    {
        var comment = _comments.FirstOrDefault(s => s.Id == taskCommentId);
        if (comment is null)
            return Result.Failure(TaskCommentErrors.NotFound);

        _comments.Remove(comment);

        return Result.Success();
    }
}
