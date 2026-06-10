using DoingTasks.SharedKernel.Domain;
using DoingTasks.SharedKernel.Results;

namespace DoingTasks.Domain.Workspaces;

public sealed class WorkspaceState : Entity
{
    public Guid WorkspaceId { get; private set; }
    public string Name { get; private set; }
    public StateOrder Order { get; private set; }

    private WorkspaceState() { }

    internal static Result<WorkspaceState> Create(Guid workspaceId, string name, StateOrder stateOrder)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<WorkspaceState>(WorkspaceStateErrors.NameRequired);               

        return Result.Success(new WorkspaceState
        {
            Id = Guid.NewGuid(),
            WorkspaceId = workspaceId,
            Name = name,
            Order = stateOrder
        });
    }

    internal Result Reorder(int order)
    {
        var newOrderResult = StateOrder.Create(order);
        if (newOrderResult.IsFailure)
            return Result.Failure<WorkspaceState>(newOrderResult.Error);

        Order = newOrderResult.Value;
        return Result.Success();
    }

    internal Result Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<WorkspaceState>(WorkspaceStateErrors.NameRequired);

        Name = name;

        return Result.Success();
    }
}
