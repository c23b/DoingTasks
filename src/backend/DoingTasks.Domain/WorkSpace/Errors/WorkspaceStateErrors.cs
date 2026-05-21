using DoingTasks.SharedKernel.Results;

namespace DoingTasks.Domain.WorkSpace;

// Domain/Workspaces/WorkspaceStateErrors.cs
public static class WorkspaceStateErrors
{
    public static readonly Error NameRequired =
        Error.Validation("WorkspaceState.NameRequired", "State name is required");

    public static readonly Error NotFound =
        Error.NotFound("WorkspaceState.NotFound", "Workspace state was not found");

    public static readonly Error TheStateAlreadyInThisOrder =
        Error.NotFound("WorkspaceState.TheStateAlreadyInThisOrder", "Workspace state already in this order");
}
