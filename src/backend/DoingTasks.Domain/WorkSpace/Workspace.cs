using DoingTasks.SharedKernel.Domain;
using DoingTasks.SharedKernel.Results;

namespace DoingTasks.Domain.WorkSpace;

public sealed class Workspace : AggregateRoot
{
    public string Name { get; private set; }
    public string? GroupName { get; private set; }  // agrupador visual no frontend
    public Guid OwnerId { get; private set; }
    public bool AllowCollaboratorEditing { get; private set; }

    private readonly List<WorkspaceState> _states = new();
    private readonly List<WorkspaceMember> _members = new();

    public IReadOnlyCollection<WorkspaceState> States => _states.AsReadOnly();
    public IReadOnlyCollection<WorkspaceMember> Members => _members.AsReadOnly();

    // INVARIANTE: mínimo 2 states para ser operacional
    public bool IsOperational => _states.Count >= 2;

    // State inicial é sempre o de menor Order
    public WorkspaceState? InitialState => _states.MinBy(s => s.Order.Value);

    private Workspace() { }

    public static Result<Workspace> Create(string name, Guid ownerId, string? groupName = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<Workspace>(WorkspaceErrors.NameRequired);

        var workspace = new Workspace
        {
            Id = Guid.NewGuid(),
            Name = name,
            GroupName = groupName,
            OwnerId = ownerId,
            AllowCollaboratorEditing = false
        };

        workspace._members.Add(WorkspaceMember.Create(ownerId, MemberRole.Owner));
        workspace.RaiseDomainEvent(new WorkspaceCreatedDomainEvent(workspace.Id, ownerId));

        return Result.Success(workspace);
    }

    public Result AddState(string name, int order)
    {
        // INVARIANTE: sem order duplicado
        if (_states.Any(s => s.Order.Value == order))
            return Result.Failure(WorkspaceErrors.DuplicateStateOrder);

        // INVARIANTE: sequência sem gaps
        var maxOrder = _states.Any() ? _states.Max(s => s.Order.Value) : 0;
        if (order != maxOrder + 1)
            return Result.Failure(WorkspaceErrors.StateOrderGap);

        var stateResult = WorkspaceState.Create(name, order);
        if (stateResult.IsFailure)
            return Result.Failure(stateResult.Error);

        _states.Add(stateResult.Value);
        RaiseDomainEvent(new WorkspaceStateAddedDomainEvent(Id, stateResult.Value.Id));
        return Result.Success();
    }

    public Result InviteMember(Guid userId, Guid requesterId)
    {
        if (requesterId != OwnerId)
            return Result.Failure(WorkspaceErrors.OnlyOwnerCanInvite);

        if (_members.Any(m => m.UserId == userId))
            return Result.Failure(WorkspaceErrors.AlreadyMember);

        _members.Add(WorkspaceMember.Create(userId, MemberRole.Collaborator));
        RaiseDomainEvent(new MemberInvitedDomainEvent(Id, userId));
        return Result.Success();
    }

    public Result ToggleCollaboratorEditing(Guid requesterId)
    {
        if (requesterId != OwnerId)
            return Result.Failure(WorkspaceErrors.OnlyOwnerCanConfigure);

        AllowCollaboratorEditing = !AllowCollaboratorEditing;
        return Result.Success();
    }

    public bool HasState(Guid stateId) => _states.Any(s => s.Id == stateId);
    public bool IsMember(Guid userId) => _members.Any(m => m.UserId == userId);
}
