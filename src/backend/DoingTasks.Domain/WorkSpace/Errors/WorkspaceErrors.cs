using DoingTasks.SharedKernel.Results;

namespace DoingTasks.Domain.WorkSpace;

// Domain/Workspaces/WorkspaceErrors.cs
public static class WorkspaceErrors
{
    public static readonly Error NameRequired =
        Error.Validation("Workspace.NameRequired", "Name is required");

    public static readonly Error GroupNameRequired =
        Error.Validation("Workspace.GroupNameRequired", "Name is required");

    public static readonly Error OnlyOwnerCanUpdate =
        Error.Problem("Workspace.OnlyOwnerCanUpdate", "Only the owner can update the workspace");

    public static readonly Error OnlyOwnerCanInvite =
        Error.Problem("Workspace.OnlyOwnerCanInvite", "Only the owner can invite members");

    public static readonly Error AlreadyMember =
        Error.Conflict("Workspace.AlreadyMember", "User is already a member of this workspace");

    public static readonly Error OnlyOwnerCanConfigure =
        Error.Problem("Workspace.OnlyOwnerCanConfigure", "Only the owner can configure this workspace");

    public static readonly Error DuplicateStateOrder =
        Error.Conflict("Workspace.DuplicateStateOrder", "A state with this order already exists");

    public static readonly Error StateOrderGap =
        Error.Validation("Workspace.StateOrderGap", "State order must be sequential without gaps");

    public static readonly Error NotFound =
        Error.NotFound("Workspace.NotFound", "Workspace was not found");

    public static readonly Error NotOperational =
        Error.Problem("Workspace.NotOperational", "Workspace requires at least 2 states to be operational");

    public static readonly Error OnlyOwnerCanAddStates =
        Error.Problem("Workspace.OnlyOwnerCanAddStates", "Only the owner can add states");

    public static readonly Error OnlyOwnerCanReorderStates =
        Error.Problem("Workspace.OnlyOwnerCanReorderStates", "Only the owner can reorder the states");

    public static readonly Error OnlyOwnerCanChangeRole =
        Error.Problem("Workspace.OnlyOwnerCanChangeRole", "Only the owner can change role of the members");

    public static readonly Error NotMember =
        Error.Problem("Workspace.NotMember", "The member is not part of this workspace");

}
