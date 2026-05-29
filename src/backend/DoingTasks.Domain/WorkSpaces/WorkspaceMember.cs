using DoingTasks.SharedKernel.Domain;
using DoingTasks.SharedKernel.Results;

namespace DoingTasks.Domain.Workspaces;

public sealed class WorkspaceMember : Entity
{
    public Guid UserId { get; private set; }
    public MemberRole Role { get; private set; }
    public DateTime JoinedAt { get; private set; }

    private WorkspaceMember() { }

    internal static WorkspaceMember Create(Guid userId, MemberRole role) =>
        new()
        {
            UserId = userId,
            Role = role,
            JoinedAt = DateTime.UtcNow
        };

    internal Result ChangeRole(MemberRole newRole)
    {
        Role = newRole;
        return Result.Success();
    }
}
