using DoingTasks.SharedKernel.Domain;
using DoingTasks.SharedKernel.Results;

namespace DoingTasks.Domain.Workspaces;

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

    public static Result<Workspace> Create(
        Guid ownerId, 
        string name, 
        string? groupName = null, 
        bool allowCollaboratorEditing = false)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<Workspace>(WorkspaceErrors.NameRequired);

        var workspace = new Workspace
        {
            OwnerId = ownerId,
            Name = name,
            GroupName = groupName,
            AllowCollaboratorEditing = allowCollaboratorEditing
        };
                
        workspace.RaiseDomainEvent(new WorkspaceCreatedDomainEvent(workspace.Id, ownerId));

        return Result.Success(workspace);
    }
        
    public Result Rename(Guid requesterId, string name)
    {
        if (requesterId != OwnerId)
            return Result.Failure(WorkspaceErrors.OnlyOwnerCanUpdate);

        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<Workspace>(WorkspaceErrors.NameRequired);

        Name = name;
        RaiseDomainEvent(new WorkspaceRenamedDomainEvent(Id, name));

        return Result.Success();
    }

    public Result Regroup(Guid requesterId, string groupName)
    {
        if (requesterId != OwnerId)
            return Result.Failure(WorkspaceErrors.OnlyOwnerCanUpdate);

        if (string.IsNullOrWhiteSpace(groupName))
            return Result.Failure<Workspace>(WorkspaceErrors.GroupNameRequired);

        GroupName = groupName;
        RaiseDomainEvent(new WorkspaceRegroupedDomainEvent(Id, groupName));

        return Result.Success();
    }

    public Result ToggleCollaboratorEditing(Guid requesterId)
    {
        if (requesterId != OwnerId)
            return Result.Failure(WorkspaceErrors.OnlyOwnerCanConfigure);

        AllowCollaboratorEditing = !AllowCollaboratorEditing;
        RaiseDomainEvent(new CollaboratorEditingToggledDomainEvent(Id, AllowCollaboratorEditing));
        return Result.Success();
    }

    public Result AddState(Guid requesterId, string name, int order)
    {
        if (requesterId != OwnerId)
            return Result.Failure(WorkspaceErrors.OnlyOwnerCanAddStates);

        bool reorder = _states.Any() ? _states.Any(s => s.Order.Value == order) : false;

        // INVARIANTE: sequência sem gaps
        var maxOrder = _states.Any() ? _states.Max(s => s.Order.Value) : 0;
        if (reorder is false && order != maxOrder + 1)
            return Result.Failure(WorkspaceErrors.StateOrderGap);

        var stateResult = WorkspaceState.Create(name, order);
        if (stateResult.IsFailure)
            return Result.Failure(stateResult.Error);

        if (reorder)
            Reorder(requesterId, stateResult.Value);

        _states.Add(stateResult.Value);
        RaiseDomainEvent(new WorkspaceStateAddedDomainEvent(Id, stateResult.Value.Id));
        return Result.Success();
    }

    public Result RemoveState(Guid requesterId, Guid stateId)
    {
        if (requesterId != OwnerId)
            return Result.Failure(WorkspaceErrors.OnlyOwnerCanAddStates);

        var state = _states.SingleOrDefault(s => s.Id == stateId);

        if (state is null)
            return Result.Failure(WorkspaceStateErrors.NotFound);

        var maxOrder = _states.Any() ? _states.Max(s => s.Order.Value) : 0;
        bool reorder = state.Order.Value != maxOrder;
        
        _states.Remove(state);

        if (reorder)
            Reorder(requesterId, state, true);

        RaiseDomainEvent(new WorkspaceStateRemovedDomainEvent(Id, state.Id));
        return Result.Success();
    }

    public Result Reorder(Guid requesterId, Guid stateId, int newOrder)
    {
        if (requesterId != OwnerId)
            return Result.Failure(WorkspaceErrors.OnlyOwnerCanReorderStates);

        var state = _states.SingleOrDefault(s => s.Id == stateId);
        if (state is null)
            return Result.Failure(WorkspaceStateErrors.NotFound);

        if (state.Order.Value == newOrder)
            return Result.Failure(WorkspaceStateErrors.TheStateAlreadyInThisOrder);

        var reorderResult = state.Reorder(newOrder);
        if (reorderResult.IsFailure)
            return Result.Failure(reorderResult.Error);

        Reorder(requesterId, state);
        RaiseDomainEvent(new WorkspaceStateReorderedDomainEvent(Id, stateId, newOrder));

        return Result.Success();
    }

    private void Reorder(
        Guid requesterId, 
        WorkspaceState workspaceState, 
        bool isReorderAfterRemoval = false)
    {
        int increase = isReorderAfterRemoval is true ? -1 : 1;
                
        foreach (var state in _states.Where(s => s.Id != workspaceState.Id 
                                              && s.Order.Value >= workspaceState.Order.Value))
                state.Reorder(state.Order.Value + increase);
    }

    public Result RenameState(Guid requesterId, Guid stateId, string name)
    {
        if (requesterId != OwnerId)
            return Result.Failure(WorkspaceErrors.OnlyOwnerCanConfigure);

        var state = _states.SingleOrDefault(s => s.Id == stateId);
        if (state is null)
            return Result.Failure(WorkspaceStateErrors.NotFound);

        var renameResult = state.Rename(name);
        if (renameResult.IsSuccess)
            RaiseDomainEvent(new WorkspaceStateRenamedDomainEvent(Id, stateId, name));

        return renameResult;
    }

    public Result InviteMember(Guid userId, Guid requesterId, MemberRole memberRole)
    {
        if (requesterId != OwnerId)
            return Result.Failure(WorkspaceErrors.OnlyOwnerCanInvite);

        if (_members.Any(m => m.UserId == userId))
            return Result.Failure(WorkspaceErrors.AlreadyMember);

        _members.Add(WorkspaceMember.Create(userId, memberRole));
        RaiseDomainEvent(new MemberInvitedDomainEvent(Id, userId));
        return Result.Success();
    }

    public Result ChangeRoleOfMember(Guid userId, Guid requesterId, MemberRole newRole)
    {
        if (requesterId != OwnerId)
            return Result.Failure(WorkspaceErrors.OnlyOwnerCanChangeRole);

        var member = _members.SingleOrDefault(m => m.UserId == userId);
        if (member is null)
            return Result.Failure(WorkspaceErrors.NotMember);

        var changeRoleResult = member.ChangeRole(newRole);
        if (changeRoleResult.IsSuccess)
            RaiseDomainEvent(new MemberRoleChangedDomainEvent(Id, userId, newRole));

        return changeRoleResult;
    }

    public Result RemoveMember(Guid userId, Guid requesterId)
    {
        if (requesterId != OwnerId)
            return Result.Failure(WorkspaceErrors.OnlyOwnerCanChangeRole);

        var member = _members.SingleOrDefault(m => m.UserId == userId);
        if (member is null)
            return Result.Failure(WorkspaceErrors.NotMember);

        _members.Remove(member);

        return Result.Success();
    }

    public bool HasState(Guid stateId) => _states.Any(s => s.Id == stateId);

    public bool IsMember(Guid userId) => _members.Any(m => m.UserId == userId);
}
