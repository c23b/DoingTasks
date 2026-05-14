using DoingTasks.Domain.WorkSpace.ValueObjects;
using DoingTasks.SharedKernel.Domain;
using DoingTasks.SharedKernel.Results;

namespace DoingTasks.Domain.WorkSpace;

// ─── WORKSPACE ───────────────────────────────────────────

public sealed class WorkspaceState : Entity
{
    public string Name { get; private set; }
    public StateOrder Order { get; private set; }

    private WorkspaceState() { }

    internal static Result<WorkspaceState> Create(string name, int order)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<WorkspaceState>(WorkspaceStateErrors.NameRequired);

        var orderResult = StateOrder.Create(order);
        if (orderResult.IsFailure)
            return Result.Failure<WorkspaceState>(orderResult.Error);

        return Result.Success(new WorkspaceState
        {
            Id = Guid.NewGuid(),
            Name = name,
            Order = orderResult.Value
        });
    }
}
