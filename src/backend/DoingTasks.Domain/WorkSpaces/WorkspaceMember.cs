using DoingTasks.SharedKernel.Domain;
using DoingTasks.SharedKernel.Results;

namespace DoingTasks.Domain.Workspaces;

public sealed class WorkspaceMember : Entity
{
    public Guid WorkspaceId { get; private set; }
    public Guid UserId { get; private set; }
    public MemberRole Role { get; private set; }

    private WorkspaceMember() { }

    internal static WorkspaceMember Create(Guid workspaceId, Guid userId, MemberRole role) =>
        new()
        {
            WorkspaceId = workspaceId,
            UserId = userId,
            Role = role,
        };

    internal Result ChangeRole(MemberRole newRole)
    {
        Role = newRole;
        return Result.Success();
    }
}
