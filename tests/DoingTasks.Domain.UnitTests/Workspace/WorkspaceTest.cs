using DoingTasks.Domain.Workspaces;

namespace DoingTasks.Domain.UnitTests.Workspaces;

/// <summary>
/// Unit tests for the <see cref="Workspace"/> aggregate root.
/// </summary>
/// <remarks>
/// Tests cover workspace creation, member management, state management, and configuration operations,
/// validating both success and failure scenarios with comprehensive error handling validation.
/// </remarks>
[Collection(nameof(WorkspaceCollection))]
public class WorkspaceTest
{
    private readonly WorkspaceTestFixture _workspaceTestFixture;

    public WorkspaceTest(WorkspaceTestFixture workspaceTestFixture)
    {
        _workspaceTestFixture = workspaceTestFixture;
    }

    #region Create Tests

    /// <summary>
    /// Tests successful creation of a workspace with valid parameters.
    /// </summary>
    /// <remarks>
    /// Verifies that when creating a workspace with valid owner ID and name,
    /// the operation succeeds and returns a workspace object with all properties correctly set.
    /// </remarks>
    [Fact(DisplayName = "Workspace - Create Success")]
    public void Workspace_Create_Success()
    {
        var ownerId = Guid.NewGuid();
        var result = Workspace.Create(ownerId, "Test Workspace", "Test Group", false);

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Value.Id);
        Assert.Equal(ownerId, result.Value.OwnerId);
        Assert.Equal("Test Workspace", result.Value.Name);
        Assert.Equal("Test Group", result.Value.GroupName);
        Assert.False(result.Value.AllowCollaboratorEditing);
    }

    /// <summary>
    /// Tests workspace creation with optional parameters omitted.
    /// </summary>
    /// <remarks>
    /// Verifies that a workspace can be created with only required parameters,
    /// with optional parameters taking their default values.
    /// </remarks>
    [Fact(DisplayName = "Workspace - Create Success With Defaults")]
    public void Workspace_Create_Success_WithDefaults()
    {
        var ownerId = Guid.NewGuid();
        var result = Workspace.Create(ownerId, "Minimal Workspace");

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(ownerId, result.Value.OwnerId);
        Assert.Equal("Minimal Workspace", result.Value.Name);
        Assert.Null(result.Value.GroupName);
        Assert.False(result.Value.AllowCollaboratorEditing);
    }

    /// <summary>
    /// Tests workspace creation failure when name is empty.
    /// </summary>
    /// <remarks>
    /// Verifies that when creating a workspace with an empty name, the operation fails
    /// and returns the appropriate <see cref="WorkspaceErrors.NameRequired"/> error.
    /// </remarks>
    [Fact(DisplayName = "Workspace - Create Error NameRequired")]
    public void Workspace_Create_Error_NameRequired()
    {
        var result = Workspace.Create(Guid.NewGuid(), string.Empty);

        Assert.NotNull(result);
        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(WorkspaceErrors.NameRequired.Code, result.Error.Code);
        Assert.Equal(WorkspaceErrors.NameRequired.Description, result.Error.Description);
    }

    /// <summary>
    /// Tests workspace creation failure when name is whitespace.
    /// </summary>
    /// <remarks>
    /// Verifies that whitespace-only names are rejected during workspace creation.
    /// </remarks>
    [Fact(DisplayName = "Workspace - Create Error NameRequired Whitespace")]
    public void Workspace_Create_Error_NameRequired_Whitespace()
    {
        var result = Workspace.Create(Guid.NewGuid(), "   ");

        Assert.NotNull(result);
        Assert.True(result.IsFailure);
        Assert.Equal(WorkspaceErrors.NameRequired.Code, result.Error.Code);
        Assert.Equal(WorkspaceErrors.NameRequired.Description, result.Error.Description);
    }

    #endregion

    #region Rename Tests

    /// <summary>
    /// Tests successful renaming of a workspace by the owner.
    /// </summary>
    /// <remarks>
    /// Verifies that when the owner renames a workspace with a valid name,
    /// the operation succeeds and the name is updated.
    /// </remarks>
    [Fact(DisplayName = "Workspace - Rename Success")]
    public void Workspace_Rename_Success()
    {
        var workspace = _workspaceTestFixture.GenerateWorkspace();
        var newName = "Updated Workspace Name";

        var result = workspace.Rename(workspace.OwnerId, newName);

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(newName, workspace.Name);
    }

    /// <summary>
    /// Tests workspace rename failure when requested by non-owner.
    /// </summary>
    /// <remarks>
    /// Verifies that only the workspace owner can rename the workspace.
    /// Non-owners receive the appropriate authorization error.
    /// </remarks>
    [Fact(DisplayName = "Workspace - Rename Error OnlyOwnerCanUpdate")]
    public void Workspace_Rename_Error_OnlyOwnerCanUpdate()
    {
        var workspace = _workspaceTestFixture.GenerateWorkspace();
        var nonOwnerId = Guid.NewGuid();

        var result = workspace.Rename(nonOwnerId, "New Name");

        Assert.NotNull(result);
        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(WorkspaceErrors.OnlyOwnerCanUpdate.Code, result.Error.Code);
        Assert.Equal(WorkspaceErrors.OnlyOwnerCanUpdate.Description, result.Error.Description);
    }

    /// <summary>
    /// Tests workspace rename failure when new name is empty.
    /// </summary>
    /// <remarks>
    /// Verifies that a workspace cannot be renamed to an empty string.
    /// </remarks>
    [Fact(DisplayName = "Workspace - Rename Error NameRequired")]
    public void Workspace_Rename_Error_NameRequired()
    {
        var workspace = _workspaceTestFixture.GenerateWorkspace();

        var result = workspace.Rename(workspace.OwnerId, string.Empty);

        Assert.NotNull(result);
        Assert.True(result.IsFailure);
        Assert.Equal(WorkspaceErrors.NameRequired.Code, result.Error.Code);
        Assert.Equal(WorkspaceErrors.NameRequired.Description, result.Error.Description);
    }

    #endregion

    #region Regroup Tests

    /// <summary>
    /// Tests successful regrouping of a workspace by the owner.
    /// </summary>
    /// <remarks>
    /// Verifies that when the owner changes the group name of a workspace,
    /// the operation succeeds and the group name is updated.
    /// </remarks>
    [Fact(DisplayName = "Workspace - Regroup Success")]
    public void Workspace_Regroup_Success()
    {
        var workspace = _workspaceTestFixture.GenerateWorkspace();
        var newGroupName = "New Group";

        var result = workspace.Regroup(workspace.OwnerId, newGroupName);

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(newGroupName, workspace.GroupName);
    }

    /// <summary>
    /// Tests workspace regroup failure when requested by non-owner.
    /// </summary>
    /// <remarks>
    /// Verifies that only the workspace owner can change the group name.
    /// </remarks>
    [Fact(DisplayName = "Workspace - Regroup Error OnlyOwnerCanUpdate")]
    public void Workspace_Regroup_Error_OnlyOwnerCanUpdate()
    {
        var workspace = _workspaceTestFixture.GenerateWorkspace();

        var result = workspace.Regroup(Guid.NewGuid(), "New Group");

        Assert.NotNull(result);
        Assert.True(result.IsFailure);
        Assert.Equal(WorkspaceErrors.OnlyOwnerCanUpdate.Code, result.Error.Code);
        Assert.Equal(WorkspaceErrors.OnlyOwnerCanUpdate.Description, result.Error.Description);
    }

    /// <summary>
    /// Tests workspace regroup failure when group name is empty.
    /// </summary>
    /// <remarks>
    /// Verifies that a workspace group name cannot be set to an empty string.
    /// </remarks>
    [Fact(DisplayName = "Workspace - Regroup Error GroupNameRequired")]
    public void Workspace_Regroup_Error_GroupNameRequired()
    {
        var workspace = _workspaceTestFixture.GenerateWorkspace();

        var result = workspace.Regroup(workspace.OwnerId, string.Empty);

        Assert.NotNull(result);
        Assert.True(result.IsFailure);
        Assert.Equal(WorkspaceErrors.GroupNameRequired.Code, result.Error.Code);
        Assert.Equal(WorkspaceErrors.GroupNameRequired.Description, result.Error.Description);
    }

    #endregion

    #region Toggle Collaborator Editing Tests

    /// <summary>
    /// Tests successful toggling of collaborator editing permission.
    /// </summary>
    /// <remarks>
    /// Verifies that the owner can toggle the collaborator editing setting,
    /// and the setting is correctly inverted.
    /// </remarks>
    [Fact(DisplayName = "Workspace - ToggleCollaboratorEditing Success")]
    public void Workspace_ToggleCollaboratorEditing_Success()
    {
        var workspace = _workspaceTestFixture.GenerateWorkspace(Guid.NewGuid(), "Test", allowCollaboratorEditing: false);
        var initialState = workspace.AllowCollaboratorEditing;

        var result = workspace.ToggleCollaboratorEditing(workspace.OwnerId);

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.NotEqual(initialState, workspace.AllowCollaboratorEditing);
        Assert.True(workspace.AllowCollaboratorEditing);
    }

    /// <summary>
    /// Tests toggle collaborator editing failure when requested by non-owner.
    /// </summary>
    /// <remarks>
    /// Verifies that only the workspace owner can toggle collaborator editing permission.
    /// </remarks>
    [Fact(DisplayName = "Workspace - ToggleCollaboratorEditing Error OnlyOwnerCanConfigure")]
    public void Workspace_ToggleCollaboratorEditing_Error_OnlyOwnerCanConfigure()
    {
        var workspace = _workspaceTestFixture.GenerateWorkspace();

        var result = workspace.ToggleCollaboratorEditing(Guid.NewGuid());

        Assert.NotNull(result);
        Assert.True(result.IsFailure);
        Assert.Equal(WorkspaceErrors.OnlyOwnerCanConfigure.Code, result.Error.Code);
        Assert.Equal(WorkspaceErrors.OnlyOwnerCanConfigure.Description, result.Error.Description);
    }

    #endregion

    #region AddState Tests

    /// <summary>
    /// Tests successful addition of a state to the workspace.
    /// </summary>
    /// <remarks>
    /// Verifies that the owner can add a new state with the correct sequential order.
    /// </remarks>
    [Fact(DisplayName = "Workspace - AddState Success")]
    public void Workspace_AddState_Success()
    {
        var workspace = _workspaceTestFixture.GenerateWorkspace();

        var result = workspace.AddState(workspace.OwnerId, "Todo", 1);

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Single(workspace.States);
        Assert.Equal("Todo", workspace.States.First().Name);
        Assert.Equal(1, workspace.States.First().Order.Value);
    }

    /// <summary>
    /// Tests successful addition of a state to the workspace forcing reorder.
    /// </summary>
    /// <remarks>
    /// Verifies that the owner can add a new state with the correct sequential order forcing reorder.
    /// </remarks>
    [Fact(DisplayName = "Workspace - AddState Force Reorder Success")]
    public void Workspace_AddState_Force_Reorder_Success()
    {
        var workspace = _workspaceTestFixture.GenerateWorkspace();

        var fistStateResult = workspace.AddState(workspace.OwnerId, "Todo", 1);
        var result = workspace.AddState(workspace.OwnerId, "Backlog", 1);

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Contains(workspace.States, s => s.Name == "Backlog" 
                                            && s.Order.Value == 1);
    }

    /// <summary>
    /// Tests add state failure when requested by non-owner.
    /// </summary>
    /// <remarks>
    /// Verifies that only the workspace owner can add states.
    /// </remarks>
    [Fact(DisplayName = "Workspace - AddState Error OnlyOwnerCanAddStates")]
    public void Workspace_AddState_Error_OnlyOwnerCanAddStates()
    {
        var workspace = _workspaceTestFixture.GenerateWorkspace();

        var result = workspace.AddState(Guid.NewGuid(), "Todo", 1);

        Assert.NotNull(result);
        Assert.True(result.IsFailure);
        Assert.Equal(WorkspaceErrors.OnlyOwnerCanAddStates.Code, result.Error.Code);
        Assert.Equal(WorkspaceErrors.OnlyOwnerCanAddStates.Description, result.Error.Description);
    }

    /// <summary>
    /// Tests add state failure when state name is empty.
    /// </summary>
    /// <remarks>
    /// Verifies that a state cannot be added without a valid name.
    /// </remarks>
    [Fact(DisplayName = "Workspace - AddState Error NameRequired")]
    public void Workspace_AddState_Error_NameRequired()
    {
        var workspace = _workspaceTestFixture.GenerateWorkspace();

        var result = workspace.AddState(workspace.OwnerId, string.Empty, 1);

        Assert.NotNull(result);
        Assert.True(result.IsFailure);
        Assert.Equal(WorkspaceStateErrors.NameRequired.Code, result.Error.Code);
        Assert.Equal(WorkspaceStateErrors.NameRequired.Description, result.Error.Description);
    }

    /// <summary>
    /// Tests add state failure when order has a gap in sequence.
    /// </summary>
    /// <remarks>
    /// Verifies that states must be added with sequential order without gaps.
    /// </remarks>
    [Fact(DisplayName = "Workspace - AddState Error StateOrderGap")]
    public void Workspace_AddState_Error_StateOrderGap()
    {
        var workspace = _workspaceTestFixture.GenerateWorkspace();
        workspace.AddState(workspace.OwnerId, "Todo", 1);

        var result = workspace.AddState(workspace.OwnerId, "InProgress", 3); // Gap: should be 2

        Assert.NotNull(result);
        Assert.True(result.IsFailure);
        Assert.Equal(WorkspaceErrors.StateOrderGap.Code, result.Error.Code);
        Assert.Equal(WorkspaceErrors.StateOrderGap.Description, result.Error.Description);
    }

    /// <summary>
    /// Tests add state failure when order is invalid (less than 1).
    /// </summary>
    /// <remarks>
    /// Verifies that state order must be at least 1.
    /// </remarks>
    [Fact(DisplayName = "Workspace - AddState Error InvalidOrder")]
    public void Workspace_AddState_Error_InvalidOrder()
    {
        var workspace = _workspaceTestFixture.GenerateWorkspace();

        var result = workspace.AddState(workspace.OwnerId, "Todo", 0);

        Assert.NotNull(result);
        Assert.True(result.IsFailure);
        Assert.Equal(StateOrderErrors.Invalid.Code, result.Error.Code);
        Assert.Equal(StateOrderErrors.Invalid.Description, result.Error.Description);
    }

    #endregion

    #region RemoveState Tests

    /// <summary>
    /// Tests successful removal of a state from the workspace.
    /// </summary>
    /// <remarks>
    /// Verifies that the owner can remove an existing state from the workspace.
    /// </remarks>
    [Fact(DisplayName = "Workspace - RemoveState Success")]
    public void Workspace_RemoveState_Success()
    {
        var workspace = _workspaceTestFixture.GenerateWorkspace();
        workspace.AddState(workspace.OwnerId, "Todo", 1);
        var stateId = workspace.States.First().Id;

        var result = workspace.RemoveState(workspace.OwnerId, stateId);

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Empty(workspace.States);
    }

    /// <summary>
    /// Tests successful removal of a state from the workspace forcing reorder.
    /// </summary>
    /// <remarks>
    /// Verifies that the owner can remove an existing state from the workspace forcing reorder.
    /// </remarks>
    [Fact(DisplayName = "Workspace - RemoveState Force Reorder Success")]
    public void Workspace_RemoveState_Force_Reorder_Success()
    {
        var workspace = _workspaceTestFixture.GenerateWorkspace();
        workspace.AddState(workspace.OwnerId, "Todo", 1);
        workspace.AddState(workspace.OwnerId, "Doing", 2);
        var stateId = workspace.States.Where(s => s.Order.Value == 1).First().Id;

        var result = workspace.RemoveState(workspace.OwnerId, stateId);

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Single(workspace.States);
        Assert.Contains(workspace.States, s => s.Name == "Doing" 
                                            && s.Order.Value == 1);
    }

    /// <summary>
    /// Tests remove state failure when requested by non-owner.
    /// </summary>
    /// <remarks>
    /// Verifies that only the workspace owner can remove states.
    /// </remarks>
    [Fact(DisplayName = "Workspace - RemoveState Error OnlyOwnerCanAddStates")]
    public void Workspace_RemoveState_Error_OnlyOwnerCanAddStates()
    {
        var workspace = _workspaceTestFixture.GenerateWorkspace();
        workspace.AddState(workspace.OwnerId, "Todo", 1);
        var stateId = workspace.States.First().Id;

        var result = workspace.RemoveState(Guid.NewGuid(), stateId);

        Assert.NotNull(result);
        Assert.True(result.IsFailure);
        Assert.Equal(WorkspaceErrors.OnlyOwnerCanAddStates.Code, result.Error.Code);
        Assert.Equal(WorkspaceErrors.OnlyOwnerCanAddStates.Description, result.Error.Description);
    }

    /// <summary>
    /// Tests remove state failure when state does not exist.
    /// </summary>
    /// <remarks>
    /// Verifies that attempting to remove a non-existent state returns NotFound error.
    /// </remarks>
    [Fact(DisplayName = "Workspace - RemoveState Error NotFound")]
    public void Workspace_RemoveState_Error_NotFound()
    {
        var workspace = _workspaceTestFixture.GenerateWorkspace();

        var result = workspace.RemoveState(workspace.OwnerId, Guid.NewGuid());

        Assert.NotNull(result);
        Assert.True(result.IsFailure);
        Assert.Equal(WorkspaceStateErrors.NotFound.Code, result.Error.Code);
        Assert.Equal(WorkspaceStateErrors.NotFound.Description, result.Error.Description);
    }

    #endregion

    #region Reorder State Tests

    /// <summary>
    /// Tests successful reordering of workspace states.
    /// </summary>
    /// <remarks>
    /// Verifies that the owner can reorder existing states to different positions.
    /// </remarks>
    [Fact(DisplayName = "Workspace - Reorder Success")]
    public void Workspace_Reorder_Success()
    {
        var workspace = _workspaceTestFixture.GenerateWorkspace();
        workspace.AddState(workspace.OwnerId, "Todo", 1);
        workspace.AddState(workspace.OwnerId, "InProgress", 2);
        var todoStateId = workspace.States.First(s => s.Name == "Todo").Id;

        var result = workspace.Reorder(workspace.OwnerId, todoStateId, 2);

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
    }

    /// <summary>
    /// Tests reorder failure when requested by non-owner.
    /// </summary>
    /// <remarks>
    /// Verifies that only the workspace owner can reorder states.
    /// </remarks>
    [Fact(DisplayName = "Workspace - Reorder Error OnlyOwnerCanReorderStates")]
    public void Workspace_Reorder_Error_OnlyOwnerCanReorderStates()
    {
        var workspace = _workspaceTestFixture.GenerateWorkspace();
        workspace.AddState(workspace.OwnerId, "Todo", 1);
        var stateId = workspace.States.First().Id;

        var result = workspace.Reorder(Guid.NewGuid(), stateId, 2);

        Assert.NotNull(result);
        Assert.True(result.IsFailure);
        Assert.Equal(WorkspaceErrors.OnlyOwnerCanReorderStates.Code, result.Error.Code);
        Assert.Equal(WorkspaceErrors.OnlyOwnerCanReorderStates.Description, result.Error.Description);
    }

    /// <summary>
    /// Tests reorder failure when state does not exist.
    /// </summary>
    /// <remarks>
    /// Verifies that attempting to reorder a non-existent state returns NotFound error.
    /// </remarks>
    [Fact(DisplayName = "Workspace - Reorder Error NotFound")]
    public void Workspace_Reorder_Error_NotFound()
    {
        var workspace = _workspaceTestFixture.GenerateWorkspace();

        var result = workspace.Reorder(workspace.OwnerId, Guid.NewGuid(), 1);

        Assert.NotNull(result);
        Assert.True(result.IsFailure);
        Assert.Equal(WorkspaceStateErrors.NotFound.Code, result.Error.Code);
        Assert.Equal(WorkspaceStateErrors.NotFound.Description, result.Error.Description);
    }

    /// <summary>
    /// Tests reorder failure when state is already at the target order.
    /// </summary>
    /// <remarks>
    /// Verifies that reordering to the same position returns an appropriate error.
    /// </remarks>
    [Fact(DisplayName = "Workspace - Reorder Error TheStateAlreadyInThisOrder")]
    public void Workspace_Reorder_Error_TheStateAlreadyInThisOrder()
    {
        var workspace = _workspaceTestFixture.GenerateWorkspace();
        workspace.AddState(workspace.OwnerId, "Todo", 1);
        var stateId = workspace.States.First().Id;

        var result = workspace.Reorder(workspace.OwnerId, stateId, 1);

        Assert.NotNull(result);
        Assert.True(result.IsFailure);
        Assert.Equal(WorkspaceStateErrors.TheStateAlreadyInThisOrder.Code, result.Error.Code);
        Assert.Equal(WorkspaceStateErrors.TheStateAlreadyInThisOrder.Description, result.Error.Description);
    }

    /// <summary>
    /// Tests reorder failure when new order is invalid.
    /// </summary>
    /// <remarks>
    /// Verifies that only the workspace owner can reorder states.
    /// </remarks>
    [Fact(DisplayName = "Workspace - Reorder Error StateOrderInvalid")]
    public void Workspace_Reorder_Error_StateOrderInvalid()
    {
        var workspace = _workspaceTestFixture.GenerateWorkspace();
        workspace.AddState(workspace.OwnerId, "Todo", 1);
        var stateId = workspace.States.First().Id;

        var result = workspace.Reorder(workspace.OwnerId, stateId, 0);

        Assert.NotNull(result);
        Assert.True(result.IsFailure);
        Assert.Equal(StateOrderErrors.Invalid.Code, result.Error.Code);
        Assert.Equal(StateOrderErrors.Invalid.Description, result.Error.Description);
    }

    #endregion

    #region RenameState Tests

    /// <summary>
    /// Tests successful renaming of a workspace state.
    /// </summary>
    /// <remarks>
    /// Verifies that the owner can rename an existing state.
    /// </remarks>
    [Fact(DisplayName = "Workspace - RenameState Success")]
    public void Workspace_RenameState_Success()
    {
        var workspace = _workspaceTestFixture.GenerateWorkspace();
        workspace.AddState(workspace.OwnerId, "Todo", 1);
        var stateId = workspace.States.First().Id;
        var newName = "To Do";

        var result = workspace.RenameState(workspace.OwnerId, stateId, newName);

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(newName, workspace.States.First().Name);
    }

    /// <summary>
    /// Tests rename state failure when requested by non-owner.
    /// </summary>
    /// <remarks>
    /// Verifies that only the workspace owner can rename states.
    /// </remarks>
    [Fact(DisplayName = "Workspace - RenameState Error OnlyOwnerCanConfigure")]
    public void Workspace_RenameState_Error_OnlyOwnerCanConfigure()
    {
        var workspace = _workspaceTestFixture.GenerateWorkspace();
        workspace.AddState(workspace.OwnerId, "Todo", 1);
        var stateId = workspace.States.First().Id;

        var result = workspace.RenameState(Guid.NewGuid(), stateId, "New Name");

        Assert.NotNull(result);
        Assert.True(result.IsFailure);
        Assert.Equal(WorkspaceErrors.OnlyOwnerCanConfigure.Code, result.Error.Code);
        Assert.Equal(WorkspaceErrors.OnlyOwnerCanConfigure.Description, result.Error.Description);
    }

    /// <summary>
    /// Tests rename state failure when state does not exist.
    /// </summary>
    /// <remarks>
    /// Verifies that attempting to rename a non-existent state returns NotFound error.
    /// </remarks>
    [Fact(DisplayName = "Workspace - RenameState Error NotFound")]
    public void Workspace_RenameState_Error_NotFound()
    {
        var workspace = _workspaceTestFixture.GenerateWorkspace();

        var result = workspace.RenameState(workspace.OwnerId, Guid.NewGuid(), "New Name");

        Assert.NotNull(result);
        Assert.True(result.IsFailure);
        Assert.Equal(WorkspaceStateErrors.NotFound.Code, result.Error.Code);
        Assert.Equal(WorkspaceStateErrors.NotFound.Description, result.Error.Description);
    }

    /// <summary>
    /// Tests rename state failure when new name is empty.
    /// </summary>
    /// <remarks>
    /// Verifies that a state cannot be renamed to an empty string.
    /// </remarks>
    [Fact(DisplayName = "Workspace - RenameState Error NameRequired")]
    public void Workspace_RenameState_Error_NameRequired()
    {
        var workspace = _workspaceTestFixture.GenerateWorkspace();
        workspace.AddState(workspace.OwnerId, "Todo", 1);
        var stateId = workspace.States.First().Id;

        var result = workspace.RenameState(workspace.OwnerId, stateId, string.Empty);

        Assert.NotNull(result);
        Assert.True(result.IsFailure);
        Assert.Equal(WorkspaceStateErrors.NameRequired.Code, result.Error.Code);
        Assert.Equal(WorkspaceStateErrors.NameRequired.Description, result.Error.Description);
    }

    #endregion

    #region Member Management Tests

    /// <summary>
    /// Tests successful invitation of a member to the workspace.
    /// </summary>
    /// <remarks>
    /// Verifies that the owner can invite a new user as a member with a specified role.
    /// </remarks>
    [Fact(DisplayName = "Workspace - InviteMember Success")]
    public void Workspace_InviteMember_Success()
    {
        var workspace = _workspaceTestFixture.GenerateWorkspace();
        var userId = Guid.NewGuid();

        var result = workspace.InviteMember(userId, workspace.OwnerId, MemberRole.Collaborator);

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Single(workspace.Members);
        Assert.Equal(userId, workspace.Members.First().UserId);
        Assert.Equal(MemberRole.Collaborator, workspace.Members.First().Role);
    }

    /// <summary>
    /// Tests invite member failure when requested by non-owner.
    /// </summary>
    /// <remarks>
    /// Verifies that only the workspace owner can invite members.
    /// </remarks>
    [Fact(DisplayName = "Workspace - InviteMember Error OnlyOwnerCanInvite")]
    public void Workspace_InviteMember_Error_OnlyOwnerCanInvite()
    {
        var workspace = _workspaceTestFixture.GenerateWorkspace();

        var result = workspace.InviteMember(Guid.NewGuid(), Guid.NewGuid(), MemberRole.Collaborator);

        Assert.NotNull(result);
        Assert.True(result.IsFailure);
        Assert.Equal(WorkspaceErrors.OnlyOwnerCanInvite.Code, result.Error.Code);
        Assert.Equal(WorkspaceErrors.OnlyOwnerCanInvite.Description, result.Error.Description);
    }

    /// <summary>
    /// Tests invite member failure when user is already a member.
    /// </summary>
    /// <remarks>
    /// Verifies that a user cannot be invited if they are already a member of the workspace.
    /// </remarks>
    [Fact(DisplayName = "Workspace - InviteMember Error AlreadyMember")]
    public void Workspace_InviteMember_Error_AlreadyMember()
    {
        var workspace = _workspaceTestFixture.GenerateWorkspace();
        var userId = Guid.NewGuid();
        workspace.InviteMember(userId, workspace.OwnerId, MemberRole.Collaborator);

        var result = workspace.InviteMember(userId, workspace.OwnerId, MemberRole.Viewer);

        Assert.NotNull(result);
        Assert.True(result.IsFailure);
        Assert.Equal(WorkspaceErrors.AlreadyMember.Code, result.Error.Code);
        Assert.Equal(WorkspaceErrors.AlreadyMember.Description, result.Error.Description);
    }

    #endregion

    #region Change Member Role Tests

    /// <summary>
    /// Tests successful change of a member's role.
    /// </summary>
    /// <remarks>
    /// Verifies that the owner can change a member's role from Collaborator to Viewer and vice versa.
    /// </remarks>
    [Fact(DisplayName = "Workspace - ChangeRoleOfMember Success")]
    public void Workspace_ChangeRoleOfMember_Success()
    {
        var workspace = _workspaceTestFixture.GenerateWorkspace();
        var userId = Guid.NewGuid();
        workspace.InviteMember(userId, workspace.OwnerId, MemberRole.Collaborator);

        var result = workspace.ChangeRoleOfMember(userId, workspace.OwnerId, MemberRole.Viewer);

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(MemberRole.Viewer, workspace.Members.First().Role);
    }

    /// <summary>
    /// Tests change role failure when requested by non-owner.
    /// </summary>
    /// <remarks>
    /// Verifies that only the workspace owner can change member roles.
    /// </remarks>
    [Fact(DisplayName = "Workspace - ChangeRoleOfMember Error OnlyOwnerCanChangeRole")]
    public void Workspace_ChangeRoleOfMember_Error_OnlyOwnerCanChangeRole()
    {
        var workspace = _workspaceTestFixture.GenerateWorkspace();
        var userId = Guid.NewGuid();
        workspace.InviteMember(userId, workspace.OwnerId, MemberRole.Collaborator);

        var result = workspace.ChangeRoleOfMember(userId, Guid.NewGuid(), MemberRole.Viewer);

        Assert.NotNull(result);
        Assert.True(result.IsFailure);
        Assert.Equal(WorkspaceErrors.OnlyOwnerCanChangeRole.Code, result.Error.Code);
        Assert.Equal(WorkspaceErrors.OnlyOwnerCanChangeRole.Description, result.Error.Description);
    }

    /// <summary>
    /// Tests change role failure when member does not exist.
    /// </summary>
    /// <remarks>
    /// Verifies that attempting to change role of a non-existent member returns NotMember error.
    /// </remarks>
    [Fact(DisplayName = "Workspace - ChangeRoleOfMember Error NotMember")]
    public void Workspace_ChangeRoleOfMember_Error_NotMember()
    {
        var workspace = _workspaceTestFixture.GenerateWorkspace();

        var result = workspace.ChangeRoleOfMember(Guid.NewGuid(), workspace.OwnerId, MemberRole.Viewer);

        Assert.NotNull(result);
        Assert.True(result.IsFailure);
        Assert.Equal(WorkspaceErrors.NotMember.Code, result.Error.Code);
        Assert.Equal(WorkspaceErrors.NotMember.Description, result.Error.Description);
    }

    #endregion

    #region Remove Member Tests

    /// <summary>
    /// Tests successful removal of a member from the workspace.
    /// </summary>
    /// <remarks>
    /// Verifies that the owner can remove an existing member from the workspace.
    /// </remarks>
    [Fact(DisplayName = "Workspace - RemoveMember Success")]
    public void Workspace_RemoveMember_Success()
    {
        var workspace = _workspaceTestFixture.GenerateWorkspace();
        var userId = Guid.NewGuid();
        workspace.InviteMember(userId, workspace.OwnerId, MemberRole.Collaborator);

        var result = workspace.RemoveMember(userId, workspace.OwnerId);

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Empty(workspace.Members);
    }

    /// <summary>
    /// Tests remove member failure when requested by non-owner.
    /// </summary>
    /// <remarks>
    /// Verifies that only the workspace owner can remove members.
    /// </remarks>
    [Fact(DisplayName = "Workspace - RemoveMember Error OnlyOwnerCanChangeRole")]
    public void Workspace_RemoveMember_Error_OnlyOwnerCanChangeRole()
    {
        var workspace = _workspaceTestFixture.GenerateWorkspace();
        var userId = Guid.NewGuid();
        workspace.InviteMember(userId, workspace.OwnerId, MemberRole.Collaborator);

        var result = workspace.RemoveMember(userId, Guid.NewGuid());

        Assert.NotNull(result);
        Assert.True(result.IsFailure);
        Assert.Equal(WorkspaceErrors.OnlyOwnerCanChangeRole.Code, result.Error.Code);
        Assert.Equal(WorkspaceErrors.OnlyOwnerCanChangeRole.Description, result.Error.Description);
    }

    /// <summary>
    /// Tests remove member failure when member does not exist.
    /// </summary>
    /// <remarks>
    /// Verifies that attempting to remove a non-existent member returns NotMember error.
    /// </remarks>
    [Fact(DisplayName = "Workspace - RemoveMember Error NotMember")]
    public void Workspace_RemoveMember_Error_NotMember()
    {
        var workspace = _workspaceTestFixture.GenerateWorkspace();

        var result = workspace.RemoveMember(Guid.NewGuid(), workspace.OwnerId);

        Assert.NotNull(result);
        Assert.True(result.IsFailure);
        Assert.Equal(WorkspaceErrors.NotMember.Code, result.Error.Code);
        Assert.Equal(WorkspaceErrors.NotMember.Description, result.Error.Description);
    }

    #endregion

    #region Utility Method Tests

    /// <summary>
    /// Tests the HasState utility method.
    /// </summary>
    /// <remarks>
    /// Verifies that HasState correctly identifies whether a state exists in the workspace.
    /// </remarks>
    [Fact(DisplayName = "Workspace - HasState Returns True For Existing State")]
    public void Workspace_HasState_ReturnsTrue_ForExistingState()
    {
        var workspace = _workspaceTestFixture.GenerateWorkspace();
        workspace.AddState(workspace.OwnerId, "Todo", 1);
        var stateId = workspace.States.First().Id;

        var result = workspace.HasState(stateId);

        Assert.True(result);
    }

    /// <summary>
    /// Tests HasState method returns false for non-existent state.
    /// </summary>
    /// <remarks>
    /// Verifies that HasState correctly returns false when querying for a non-existent state.
    /// </remarks>
    [Fact(DisplayName = "Workspace - HasState Returns False For Non-Existent State")]
    public void Workspace_HasState_ReturnsFalse_ForNonExistentState()
    {
        var workspace = _workspaceTestFixture.GenerateWorkspace();

        var result = workspace.HasState(Guid.NewGuid());

        Assert.False(result);
    }

    /// <summary>
    /// Tests the IsMember utility method.
    /// </summary>
    /// <remarks>
    /// Verifies that IsMember correctly identifies whether a user is a member of the workspace.
    /// </remarks>
    [Fact(DisplayName = "Workspace - IsMember Returns True For Existing Member")]
    public void Workspace_IsMember_ReturnsTrue_ForExistingMember()
    {
        var workspace = _workspaceTestFixture.GenerateWorkspace();
        var userId = Guid.NewGuid();
        workspace.InviteMember(userId, workspace.OwnerId, MemberRole.Collaborator);

        var result = workspace.IsMember(userId);

        Assert.True(result);
    }

    /// <summary>
    /// Tests IsMember method returns false for non-member.
    /// </summary>
    /// <remarks>
    /// Verifies that IsMember correctly returns false when querying for a user who is not a member.
    /// </remarks>
    [Fact(DisplayName = "Workspace - IsMember Returns False For Non-Member")]
    public void Workspace_IsMember_ReturnsFalse_ForNonMember()
    {
        var workspace = _workspaceTestFixture.GenerateWorkspace();

        var result = workspace.IsMember(Guid.NewGuid());

        Assert.False(result);
    }

    #endregion

    #region Operational State Tests

    /// <summary>
    /// Tests the IsOperational property.
    /// </summary>
    /// <remarks>
    /// Verifies that a workspace requires at least 2 states to be considered operational.
    /// </remarks>
    [Fact(DisplayName = "Workspace - IsOperational False With Less Than 2 States")]
    public void Workspace_IsOperational_False_WithLessThan2States()
    {
        var workspace = _workspaceTestFixture.GenerateWorkspace();
        workspace.AddState(workspace.OwnerId, "Todo", 1);

        Assert.False(workspace.IsOperational);
    }

    /// <summary>
    /// Tests the IsOperational property with 2 states.
    /// </summary>
    /// <remarks>
    /// Verifies that a workspace becomes operational when it has at least 2 states.
    /// </remarks>
    [Fact(DisplayName = "Workspace - IsOperational True With 2 States")]
    public void Workspace_IsOperational_True_With2States()
    {
        var workspace = _workspaceTestFixture.GenerateWorkspace();
        workspace.AddState(workspace.OwnerId, "Todo", 1);
        workspace.AddState(workspace.OwnerId, "InProgress", 2);

        Assert.True(workspace.IsOperational);
    }

    #endregion

    #region Initial State Tests

    /// <summary>
    /// Tests the InitialState property.
    /// </summary>
    /// <remarks>
    /// Verifies that InitialState returns the state with the lowest order value.
    /// </remarks>
    [Fact(DisplayName = "Workspace - InitialState Returns State With Lowest Order")]
    public void Workspace_InitialState_ReturnsStateWithLowestOrder()
    {
        var workspace = _workspaceTestFixture.GenerateWorkspace();
        workspace.AddState(workspace.OwnerId, "Todo", 1);
        workspace.AddState(workspace.OwnerId, "InProgress", 2);
        workspace.AddState(workspace.OwnerId, "Done", 3);

        var initialState = workspace.InitialState;

        Assert.NotNull(initialState);
        Assert.Equal("Todo", initialState.Name);
        Assert.Equal(1, initialState.Order.Value);
    }

    /// <summary>
    /// Tests InitialState returns null when no states exist.
    /// </summary>
    /// <remarks>
    /// Verifies that InitialState correctly returns null when the workspace has no states.
    /// </remarks>
    [Fact(DisplayName = "Workspace - InitialState Returns Null When No States")]
    public void Workspace_InitialState_ReturnsNull_WhenNoStates()
    {
        var workspace = _workspaceTestFixture.GenerateWorkspace();

        var initialState = workspace.InitialState;

        Assert.Null(initialState);
    }

    #endregion
}
