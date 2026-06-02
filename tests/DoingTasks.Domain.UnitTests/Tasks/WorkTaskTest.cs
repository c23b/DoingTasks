using DoingTasks.Domain.Tasks;

namespace DoingTasks.Domain.UnitTests.Tasks;

/// <summary>
/// Unit tests for the <see cref="WorkTask"/> aggregate root.
/// </summary>
/// <remarks>
/// Tests cover work task creation, state transitions, blocking/unblocking, step management,
/// comment management, and complexity handling, validating both success and failure scenarios
/// with comprehensive error handling validation.
/// </remarks>
[Collection(nameof(WorkTaskCollection))]
public class WorkTaskTest
{
    private readonly WorkTaskTestFixture _workTaskTestFixture;

    public WorkTaskTest(WorkTaskTestFixture workTaskTestFixture)
    {
        _workTaskTestFixture = workTaskTestFixture;
    }

    #region Create Tests

    /// <summary>
    /// Tests successful creation of a work task with valid parameters.
    /// </summary>
    /// <remarks>
    /// Verifies that when creating a task with valid title, workspace ID, and state ID,
    /// the operation succeeds and returns a task object with all properties correctly set.
    /// </remarks>
    [Fact(DisplayName = "WorkTask - Create Success")]
    public void WorkTask_Create_Success()
    {
        var workspaceId = Guid.NewGuid();
        var stateId = Guid.NewGuid();
        var assignedUserId = Guid.NewGuid();

        var result = WorkTask.Create("Test Task", workspaceId, stateId, "Test Description", 5, assignedUserId, 3);

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Value.Id);
        Assert.Equal("Test Task", result.Value.Title);
        Assert.Equal("Test Description", result.Value.Description);
        Assert.Equal(workspaceId, result.Value.WorkspaceId);
        Assert.Equal(stateId, result.Value.CurrentStateId);
        Assert.Equal(5, result.Value.PlannedHours);
        Assert.Equal(assignedUserId, result.Value.AssignedUserId);
        Assert.Equal(3, result.Value.Complexity.Value);
        Assert.False(result.Value.IsBlocked);
        Assert.Equal(0, result.Value.ActualHours);
    }

    /// <summary>
    /// Tests task creation with minimal parameters.
    /// </summary>
    /// <remarks>
    /// Verifies that a task can be created with only required parameters.
    /// </remarks>
    [Fact(DisplayName = "WorkTask - Create Success With Minimal Parameters")]
    public void WorkTask_Create_Success_WithMinimalParameters()
    {
        var workspaceId = Guid.NewGuid();
        var stateId = Guid.NewGuid();

        var result = WorkTask.Create("Test Task", workspaceId, stateId);

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal("Test Task", result.Value.Title);
        Assert.Null(result.Value.Description);
        Assert.Null(result.Value.PlannedHours);
        Assert.Null(result.Value.AssignedUserId);
        Assert.Null(result.Value.Complexity.Value);
    }

    /// <summary>
    /// Tests task creation failure when title is empty.
    /// </summary>
    /// <remarks>
    /// Verifies that a task cannot be created without a valid title.
    /// </remarks>
    [Fact(DisplayName = "WorkTask - Create Error TitleRequired")]
    public void WorkTask_Create_Error_TitleRequired()
    {
        var result = WorkTask.Create(string.Empty, Guid.NewGuid(), Guid.NewGuid());

        Assert.NotNull(result);
        Assert.True(result.IsFailure);
        Assert.Equal(WorkTaskErrors.TitleRequired.Code, result.Error.Code);
        Assert.Equal(WorkTaskErrors.TitleRequired.Description, result.Error.Description);
    }

    /// <summary>
    /// Tests task creation failure when complexity is out of range.
    /// </summary>
    /// <remarks>
    /// Verifies that complexity must be between 1 and 10.
    /// </remarks>
    [Theory(DisplayName = "WorkTask - Create Error Complexity OutOfRange")]
    [InlineData(0)]
    [InlineData(11)]
    [InlineData(-1)]
    public void WorkTask_Create_Error_Complexity_OutOfRange(int complexity)
    {
        var result = WorkTask.Create("Test Task", Guid.NewGuid(), Guid.NewGuid(), complexity: complexity);

        Assert.NotNull(result);
        Assert.True(result.IsFailure);
        Assert.Equal(ComplexityErrors.OutOfRange.Code, result.Error.Code);
        Assert.Equal(ComplexityErrors.OutOfRange.Description, result.Error.Description);
    }

    #endregion

    #region Retitle Tests

    /// <summary>
    /// Tests successful retitling of a work task.
    /// </summary>
    /// <remarks>
    /// Verifies that a task title can be updated with a valid new title.
    /// </remarks>
    [Fact(DisplayName = "WorkTask - Retitle Success")]
    public void WorkTask_Retitle_Success()
    {
        var task = _workTaskTestFixture.GenerateWorkTask();
        var newTitle = "Updated Task Title";

        var result = task.Retitle(newTitle);

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(newTitle, task.Title);
    }

    /// <summary>
    /// Tests retitle failure when new title is empty.
    /// </summary>
    /// <remarks>
    /// Verifies that a task cannot be retitled to an empty string.
    /// </remarks>
    [Fact(DisplayName = "WorkTask - Retitle Error TitleRequired")]
    public void WorkTask_Retitle_Error_TitleRequired()
    {
        var task = _workTaskTestFixture.GenerateWorkTask();

        var result = task.Retitle(string.Empty);

        Assert.NotNull(result);
        Assert.True(result.IsFailure);
        Assert.Equal(WorkTaskErrors.TitleRequired.Code, result.Error.Code);
        Assert.Equal(WorkTaskErrors.TitleRequired.Description, result.Error.Description);
    }

    #endregion

    #region Update Description Tests

    /// <summary>
    /// Tests successful update of task description.
    /// </summary>
    /// <remarks>
    /// Verifies that a task description can be updated with valid content.
    /// </remarks>
    [Fact(DisplayName = "WorkTask - UpdateDescription Success")]
    public void WorkTask_UpdateDescription_Success()
    {
        var task = _workTaskTestFixture.GenerateWorkTask();
        var newDescription = "Updated description content";

        var result = task.UpdateDescription(newDescription);

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(newDescription, task.Description);
    }

    /// <summary>
    /// Tests update description failure when description is empty.
    /// </summary>
    /// <remarks>
    /// Verifies that a task description cannot be set to an empty string.
    /// </remarks>
    [Fact(DisplayName = "WorkTask - UpdateDescription Error DescriptionRequired")]
    public void WorkTask_UpdateDescription_Error_DescriptionRequired()
    {
        var task = _workTaskTestFixture.GenerateWorkTask();

        var result = task.UpdateDescription(string.Empty);

        Assert.NotNull(result);
        Assert.True(result.IsFailure);
        Assert.Equal(WorkTaskErrors.DescriptionRequired.Code, result.Error.Code);
        Assert.Equal(WorkTaskErrors.DescriptionRequired.Description, result.Error.Description);
    }

    #endregion

    #region Complexity Tests

    /// <summary>
    /// Tests successful update of task complexity.
    /// </summary>
    /// <remarks>
    /// Verifies that task complexity can be updated to a valid value.
    /// </remarks>
    [Fact(DisplayName = "WorkTask - UpdateComplexity Success")]
    public void WorkTask_UpdateComplexity_Success()
    {
        var task = _workTaskTestFixture.GenerateWorkTask();

        var result = task.UpdateComplexity(5);

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(5, task.Complexity.Value);
    }

    /// <summary>
    /// Tests update complexity failure when complexity is out of range.
    /// </summary>
    /// <remarks>
    /// Verifies that complexity must be between 1 and 10.
    /// </remarks>
    [Theory(DisplayName = "WorkTask - UpdateComplexity Error OutOfRange")]
    [InlineData(0)]
    [InlineData(11)]
    [InlineData(-5)]
    public void WorkTask_UpdateComplexity_Error_OutOfRange(int complexity)
    {
        var task = _workTaskTestFixture.GenerateWorkTask();

        var result = task.UpdateComplexity(complexity);

        Assert.NotNull(result);
        Assert.True(result.IsFailure);
        Assert.Equal(ComplexityErrors.OutOfRange.Code, result.Error.Code);
        Assert.Equal(ComplexityErrors.OutOfRange.Description, result.Error.Description);
    }

    /// <summary>
    /// Tests updating complexity to null (undefined).
    /// </summary>
    /// <remarks>
    /// Verifies that complexity can be set to undefined (null).
    /// </remarks>
    [Fact(DisplayName = "WorkTask - UpdateComplexity To Undefined")]
    public void WorkTask_UpdateComplexity_ToUndefined()
    {
        var task = _workTaskTestFixture.GenerateWorkTask();

        var result = task.UpdateComplexity(null);

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Null(task.Complexity.Value);
    }

    #endregion

    #region Planned Hours Tests

    /// <summary>
    /// Tests successful update of planned hours.
    /// </summary>
    /// <remarks>
    /// Verifies that planned hours can be updated to any valid value.
    /// </remarks>
    [Fact(DisplayName = "WorkTask - UpdatePlannedHours Success")]
    public void WorkTask_UpdatePlannedHours_Success()
    {
        var task = _workTaskTestFixture.GenerateWorkTask();

        var result = task.UpdatePlannedHours(10);

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(10, task.PlannedHours);
    }

    /// <summary>
    /// Tests setting planned hours to null.
    /// </summary>
    /// <remarks>
    /// Verifies that planned hours can be cleared.
    /// </remarks>
    [Fact(DisplayName = "WorkTask - UpdatePlannedHours To Null")]
    public void WorkTask_UpdatePlannedHours_ToNull()
    {
        var task = _workTaskTestFixture.GenerateWorkTask();

        var result = task.UpdatePlannedHours(null);

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Null(task.PlannedHours);
    }

    #endregion

    #region State Transition Tests

    /// <summary>
    /// Tests successful state transition of a task.
    /// </summary>
    /// <remarks>
    /// Verifies that a task can be transitioned to a new state when not blocked.
    /// </remarks>
    [Fact(DisplayName = "WorkTask - TransitionToState Success")]
    public void WorkTask_TransitionToState_Success()
    {
        var task = _workTaskTestFixture.GenerateWorkTask();
        var newStateId = Guid.NewGuid();
        var previousStateId = task.CurrentStateId;

        var result = task.TransitionToState(newStateId);

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(newStateId, task.CurrentStateId);
        Assert.NotEqual(previousStateId, task.CurrentStateId);
    }

    /// <summary>
    /// Tests state transition failure when task is blocked.
    /// </summary>
    /// <remarks>
    /// Verifies that a blocked task cannot transition to a new state.
    /// </remarks>
    [Fact(DisplayName = "WorkTask - TransitionToState Error TaskIsBlocked")]
    public void WorkTask_TransitionToState_Error_TaskIsBlocked()
    {
        var task = _workTaskTestFixture.GenerateWorkTask();
        task.Block("Task is blocked");
        var newStateId = Guid.NewGuid();

        var result = task.TransitionToState(newStateId);

        Assert.NotNull(result);
        Assert.True(result.IsFailure);
        Assert.Equal(WorkTaskErrors.TaskIsBlocked.Code, result.Error.Code);
        Assert.Equal(WorkTaskErrors.TaskIsBlocked.Description, result.Error.Description);
    }

    #endregion

    #region Block/Unblock Tests

    /// <summary>
    /// Tests successful blocking of a task.
    /// </summary>
    /// <remarks>
    /// Verifies that a task can be blocked with a valid justification.
    /// </remarks>
    [Fact(DisplayName = "WorkTask - Block Success")]
    public void WorkTask_Block_Success()
    {
        var task = _workTaskTestFixture.GenerateWorkTask();
        var justification = "Waiting for external dependency";

        var result = task.Block(justification);

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.True(task.IsBlocked);
        Assert.Equal(justification, task.BlockJustification);
    }

    /// <summary>
    /// Tests blocking failure when task is already blocked.
    /// </summary>
    /// <remarks>
    /// Verifies that a task cannot be blocked if it's already blocked.
    /// </remarks>
    [Fact(DisplayName = "WorkTask - Block Error AlreadyBlocked")]
    public void WorkTask_Block_Error_AlreadyBlocked()
    {
        var task = _workTaskTestFixture.GenerateWorkTask();
        task.Block("Task is blocked");

        var result = task.Block("Another justification");

        Assert.NotNull(result);
        Assert.True(result.IsFailure);
        Assert.Equal(WorkTaskErrors.AlreadyBlocked.Code, result.Error.Code);
        Assert.Equal(WorkTaskErrors.AlreadyBlocked.Description, result.Error.Description);
    }

    /// <summary>
    /// Tests blocking failure when justification is empty.
    /// </summary>
    /// <remarks>
    /// Verifies that a task cannot be blocked without a justification.
    /// </remarks>
    [Fact(DisplayName = "WorkTask - Block Error BlockJustificationRequired")]
    public void WorkTask_Block_Error_BlockJustificationRequired()
    {
        var task = _workTaskTestFixture.GenerateWorkTask();

        var result = task.Block(string.Empty);

        Assert.NotNull(result);
        Assert.True(result.IsFailure);
        Assert.Equal(WorkTaskErrors.BlockJustificationRequired.Code, result.Error.Code);
        Assert.Equal(WorkTaskErrors.BlockJustificationRequired.Description, result.Error.Description);
    }

    /// <summary>
    /// Tests successful unblocking of a task.
    /// </summary>
    /// <remarks>
    /// Verifies that a blocked task can be unblocked.
    /// </remarks>
    [Fact(DisplayName = "WorkTask - Unblock Success")]
    public void WorkTask_Unblock_Success()
    {
        var task = _workTaskTestFixture.GenerateWorkTask();
        task.Block("Task is blocked");

        var result = task.Unblock();

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.False(task.IsBlocked);
        Assert.Null(task.BlockJustification);
    }

    /// <summary>
    /// Tests unblocking failure when task is not blocked.
    /// </summary>
    /// <remarks>
    /// Verifies that a task that is not blocked cannot be unblocked.
    /// </remarks>
    [Fact(DisplayName = "WorkTask - Unblock Error AlreadyUnblocked")]
    public void WorkTask_Unblock_Error_AlreadyUnblocked()
    {
        var task = _workTaskTestFixture.GenerateWorkTask();

        var result = task.Unblock();

        Assert.NotNull(result);
        Assert.True(result.IsFailure);
        Assert.Equal(WorkTaskErrors.AlreadyUnblocked.Code, result.Error.Code);
        Assert.Equal(WorkTaskErrors.AlreadyUnblocked.Description, result.Error.Description);
    }

    #endregion

    #region Actual Hours Tests

    /// <summary>
    /// Tests successful update of actual hours.
    /// </summary>
    /// <remarks>
    /// Verifies that actual hours can be updated to a valid value.
    /// </remarks>
    [Fact(DisplayName = "WorkTask - UpdateActualHours Success")]
    public void WorkTask_UpdateActualHours_Success()
    {
        var task = _workTaskTestFixture.GenerateWorkTask();

        var result = task.UpdateActualHours(8);

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(8, task.ActualHours);
    }

    /// <summary>
    /// Tests actual hours update failure when hours are below total step hours.
    /// </summary>
    /// <remarks>
    /// Verifies that actual hours cannot be less than the sum of completed step hours.
    /// </remarks>
    [Fact(DisplayName = "WorkTask - UpdateActualHours Error ActualHoursBelowStepsTotal")]
    public void WorkTask_UpdateActualHours_Error_ActualHoursBelowStepsTotal()
    {
        var task = _workTaskTestFixture.GenerateWorkTask();
        var stateId = task.CurrentStateId;
        
        // Add a step and mark it as done
        task.AddStep("Step 1", stateId);
        var step = task.Steps.First();
        task.SetStepStatusDone(step.Id, 5);

        // Try to set actual hours below the total step hours
        var result = task.UpdateActualHours(3);

        Assert.NotNull(result);
        Assert.True(result.IsFailure);
        Assert.Equal(WorkTaskErrors.ActualHoursBelowStepsTotal.Code, result.Error.Code);
        Assert.Equal(WorkTaskErrors.ActualHoursBelowStepsTotal.Description, result.Error.Description);
    }

    #endregion

    #region Assignment Tests

    /// <summary>
    /// Tests successful assignment of a task to a user.
    /// </summary>
    /// <remarks>
    /// Verifies that a task can be assigned to a user.
    /// </remarks>
    [Fact(DisplayName = "WorkTask - Assign Success")]
    public void WorkTask_Assign_Success()
    {
        var task = _workTaskTestFixture.GenerateWorkTask();
        var userId = Guid.NewGuid();

        var result = task.Assign(userId);

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(userId, task.AssignedUserId);
    }

    #endregion

    #region Step Tests

    /// <summary>
    /// Tests successful addition of a step to a task.
    /// </summary>
    /// <remarks>
    /// Verifies that a step can be added to a task with valid parameters.
    /// </remarks>
    [Fact(DisplayName = "WorkTask - AddStep Success")]
    public void WorkTask_AddStep_Success()
    {
        var task = _workTaskTestFixture.GenerateWorkTask();
        var userId = Guid.NewGuid();

        var result = task.AddStep("Step Title", userId);

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Single(task.Steps);
        Assert.Equal("Step Title", task.Steps.First().Title);
        Assert.Equal(userId, task.Steps.First().AssignedUserId);
    }

    /// <summary>
    /// Tests adding step failure when step title is empty.
    /// </summary>
    /// <remarks>
    /// Verifies that a step cannot be added without a valid title.
    /// </remarks>
    [Fact(DisplayName = "WorkTask - AddStep Error TitleRequired")]
    public void WorkTask_AddStep_Error_TitleRequired()
    {
        var task = _workTaskTestFixture.GenerateWorkTask();

        var result = task.AddStep(string.Empty, Guid.NewGuid());

        Assert.NotNull(result);
        Assert.True(result.IsFailure);
        Assert.Equal(StepErrors.TitleRequired.Code, result.Error.Code);
        Assert.Equal(StepErrors.TitleRequired.Description, result.Error.Description);
    }

    /// <summary>
    /// Tests successful removal of a step.
    /// </summary>
    /// <remarks>
    /// Verifies that a step can be removed from a task.
    /// </remarks>
    [Fact(DisplayName = "WorkTask - RemoveStep Success")]
    public void WorkTask_RemoveStep_Success()
    {
        var task = _workTaskTestFixture.GenerateWorkTask();
        task.AddStep("Step 1", Guid.NewGuid());
        var stepId = task.Steps.First().Id;

        var result = task.RemoveStep(stepId);

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Empty(task.Steps);
    }

    /// <summary>
    /// Tests remove step failure when step does not exist.
    /// </summary>
    /// <remarks>
    /// Verifies that removing a non-existent step returns NotFound error.
    /// </remarks>
    [Fact(DisplayName = "WorkTask - RemoveStep Error NotFound")]
    public void WorkTask_RemoveStep_Error_NotFound()
    {
        var task = _workTaskTestFixture.GenerateWorkTask();

        var result = task.RemoveStep(Guid.NewGuid());

        Assert.NotNull(result);
        Assert.True(result.IsFailure);
        Assert.Equal(StepErrors.NotFound.Code, result.Error.Code);
        Assert.Equal(StepErrors.NotFound.Description, result.Error.Description);
    }

    /// <summary>
    /// Tests setting a step to pending.
    /// </summary>
    /// <remarks>
    /// Verifies that a step can be marked as pending when the task is in the corresponding state.
    /// </remarks>
    [Fact(DisplayName = "WorkTask - SetStepStatusPending Success")]
    public void WorkTask_SetStepStatusPending_Success()
    {
        var task = _workTaskTestFixture.GenerateWorkTask();
        var stateId = task.CurrentStateId;
        task.AddStep("Step 1", stateId);
        var stepId = task.Steps.First().Id;
        var setStepStatusDoing = task.SetStepStatusDoing(stepId);

        var result = task.SetStepStatusPending(stepId);

        Assert.NotNull(setStepStatusDoing);
        Assert.True(setStepStatusDoing.IsSuccess);
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(StepStatus.Pending, task.Steps.First().StepStatus);
    }

    /// <summary>
    /// Tests set step Pending failure when step does not exist.
    /// </summary>
    /// <remarks>
    /// Verifies that setting a non-existent step to Pending returns NotFound error.
    /// </remarks>
    [Fact(DisplayName = "WorkTask - SetStepStatusPending Error NotFound")]
    public void WorkTask_SetStepStatusPending_Error_NotFound()
    {
        var task = _workTaskTestFixture.GenerateWorkTask();

        var result = task.SetStepStatusPending(Guid.NewGuid());

        Assert.NotNull(result);
        Assert.True(result.IsFailure);
        Assert.Equal(StepErrors.NotFound.Code, result.Error.Code);
        Assert.Equal(StepErrors.NotFound.Description, result.Error.Description);
    }

    /// <summary>
    /// Tests set step pending failure when step is already in that status.
    /// </summary>
    /// <remarks>
    /// Verifies that setting a non-existent step to pending returns StepAlreadyStatus error.
    /// </remarks>
    [Fact(DisplayName = "WorkTask - SetStepStatusPending Error StepAlreadyStatus")]
    public void WorkTask_SetStepStatusPending_Error_StepAlreadyStatus()
    {
        var task = _workTaskTestFixture.GenerateWorkTask();
        var stateId = task.CurrentStateId;
        task.AddStep("Step 1", stateId);
        var stepId = task.Steps.First().Id;

        var result = task.SetStepStatusPending(stepId);

        Assert.NotNull(result);
        Assert.True(result.IsFailure);
        Assert.Equal(StepErrors.StepAlreadyStatus.Code, result.Error.Code);
        Assert.Equal(StepErrors.StepAlreadyStatus.Description, result.Error.Description);
    }

    /// <summary>
    /// Tests setting a step to doing.
    /// </summary>
    /// <remarks>
    /// Verifies that a step can be marked as doing when the task is in the corresponding state.
    /// </remarks>
    [Fact(DisplayName = "WorkTask - SetStepStatusDoing Success")]
    public void WorkTask_SetStepStatusDoing_Success()
    {
        var task = _workTaskTestFixture.GenerateWorkTask();
        var stateId = task.CurrentStateId;
        task.AddStep("Step 1", stateId);
        var stepId = task.Steps.First().Id;

        var result = task.SetStepStatusDoing(stepId);

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(StepStatus.Doing, task.Steps.First().StepStatus);
    }

    /// <summary>
    /// Tests set step doing failure when step does not exist.
    /// </summary>
    /// <remarks>
    /// Verifies that setting a non-existent step to doing returns NotFound error.
    /// </remarks>
    [Fact(DisplayName = "WorkTask - SetStepStatusDoing Error NotFound")]
    public void WorkTask_SetStepStatusDoing_Error_NotFound()
    {
        var task = _workTaskTestFixture.GenerateWorkTask();

        var result = task.SetStepStatusDoing(Guid.NewGuid());

        Assert.NotNull(result);
        Assert.True(result.IsFailure);
        Assert.Equal(StepErrors.NotFound.Code, result.Error.Code);
        Assert.Equal(StepErrors.NotFound.Description, result.Error.Description);
    }

    /// <summary>
    /// Tests set step doing failure when step is already in that status.
    /// </summary>
    /// <remarks>
    /// Verifies that setting a non-existent step to doing returns StepAlreadyStatus error.
    /// </remarks>
    [Fact(DisplayName = "WorkTask - SetStepStatusDoing Error StepAlreadyStatus")]
    public void WorkTask_SetStepStatusDoing_Error_StepAlreadyStatus()
    {
        var task = _workTaskTestFixture.GenerateWorkTask();
        var stateId = task.CurrentStateId;
        task.AddStep("Step 1", stateId);
        var stepId = task.Steps.First().Id;

        var setStepStatusDoing = task.SetStepStatusDoing(stepId);
        var result = task.SetStepStatusDoing(stepId);

        Assert.NotNull(setStepStatusDoing);
        Assert.True(setStepStatusDoing.IsSuccess);
        Assert.NotNull(result);
        Assert.True(result.IsFailure);
        Assert.Equal(StepErrors.StepAlreadyStatus.Code, result.Error.Code);
        Assert.Equal(StepErrors.StepAlreadyStatus.Description, result.Error.Description);
    }

    /// <summary>
    /// Tests setting a step to done.
    /// </summary>
    /// <remarks>
    /// Verifies that a step can be marked as done with hours spent when task is in the corresponding state.
    /// </remarks>
    [Fact(DisplayName = "WorkTask - SetStepStatusDone Success")]
    public void WorkTask_SetStepDone_Success()
    {
        var task = _workTaskTestFixture.GenerateWorkTask();
        var stateId = task.CurrentStateId;
        task.AddStep("Step 1", stateId);
        var stepId = task.Steps.First().Id;

        var result = task.SetStepStatusDone(stepId, 5);

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(StepStatus.Done, task.Steps.First().StepStatus);
        Assert.Equal(5, task.Steps.First().ActualHours);
        Assert.Equal(5, task.ActualHours); // Task actual hours should be updated to match total step hours
    }

    /// <summary>
    /// Tests set step done failure when step does not exist.
    /// </summary>
    /// <remarks>
    /// Verifies that marking a non-existent step as done returns NotFound error.
    /// </remarks>
    [Fact(DisplayName = "WorkTask - SetStepStatusDone Error NotFound")]
    public void WorkTask_SetStepStatusDone_Error_NotFound()
    {
        var task = _workTaskTestFixture.GenerateWorkTask();

        var result = task.SetStepStatusDone(Guid.NewGuid(), 5);

        Assert.NotNull(result);
        Assert.True(result.IsFailure);
        Assert.Equal(StepErrors.NotFound.Code, result.Error.Code);
        Assert.Equal(StepErrors.NotFound.Description, result.Error.Description);
    }

    /// <summary>
    /// Tests set step done failure when hours are negative.
    /// </summary>
    /// <remarks>
    /// Verifies that a step cannot be marked as done with negative hours.
    /// </remarks>
    [Fact(DisplayName = "WorkTask - SetStepStatusDone Error InvalidHours")]
    public void WorkTask_SetStepStatusDone_Error_InvalidHours()
    {
        var task = _workTaskTestFixture.GenerateWorkTask();
        var stateId = task.CurrentStateId;
        task.AddStep("Step 1", stateId);
        var stepId = task.Steps.First().Id;

        var result = task.SetStepStatusDone(stepId, -5);

        Assert.NotNull(result);
        Assert.True(result.IsFailure);
        Assert.Equal(StepErrors.InvalidHours.Code, result.Error.Code);
        Assert.Equal(StepErrors.InvalidHours.Description, result.Error.Description);
    }

    /// <summary>
    /// Tests set step done failure when step is already in that status.
    /// </summary>
    /// <remarks>
    /// Verifies that setting a non-existent step to done returns StepAlreadyStatus error.
    /// </remarks>
    [Fact(DisplayName = "WorkTask - SetStepStatusDone Error StepAlreadyStatus")]
    public void WorkTask_SetStepStatusDone_Error_StateNotMatching()
    {
        var task = _workTaskTestFixture.GenerateWorkTask();
        var stepStateId = Guid.NewGuid();
        var differentStateId = Guid.NewGuid();
        
        // Add step with one state
        task.AddStep("Step 1");
        var stepId = task.Steps.First().Id;

        var setStepStatusDone = task.SetStepStatusDone(stepId, 5);
        var result = task.SetStepStatusDone(stepId, 5);

        Assert.NotNull(result);
        Assert.True(result.IsFailure);
        Assert.Equal(StepErrors.StepAlreadyStatus.Code, result.Error.Code);
        Assert.Equal(StepErrors.StepAlreadyStatus.Description, result.Error.Description);
    }

    /// <summary>
    /// Tests assigning a step to a user.
    /// </summary>
    /// <remarks>
    /// Verifies that a step can be assigned to a user.
    /// </remarks>
    [Fact(DisplayName = "WorkTask - AssignStep Success")]
    public void WorkTask_AssignStep_Success()
    {
        var task = _workTaskTestFixture.GenerateWorkTask();
        task.AddStep("Step 1", Guid.NewGuid());
        var stepId = task.Steps.First().Id;
        var userId = Guid.NewGuid();

        var result = task.AssignStep(stepId, userId);

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(userId, task.Steps.First().AssignedUserId);
    }

    /// <summary>
    /// Tests assign step failure when step does not exist.
    /// </summary>
    /// <remarks>
    /// Verifies that assigning a non-existent step returns NotFound error.
    /// </remarks>
    [Fact(DisplayName = "WorkTask - AssignStep Error NotFound")]
    public void WorkTask_AssignStep_Error_NotFound()
    {
        var task = _workTaskTestFixture.GenerateWorkTask();

        var result = task.AssignStep(Guid.NewGuid(), Guid.NewGuid());

        Assert.NotNull(result);
        Assert.True(result.IsFailure);
        Assert.Equal(StepErrors.NotFound.Code, result.Error.Code);
        Assert.Equal(StepErrors.NotFound.Description, result.Error.Description);
    }

    /// <summary>
    /// Tests retitling a step.
    /// </summary>
    /// <remarks>
    /// Verifies that a step can be retitled with a valid new title.
    /// </remarks>
    [Fact(DisplayName = "WorkTask - RetitleStep Success")]
    public void WorkTask_RetitleStep_Success()
    {
        var task = _workTaskTestFixture.GenerateWorkTask();
        task.AddStep("Step 1", Guid.NewGuid());
        var stepId = task.Steps.First().Id;
        var newTitle = "Updated Step Title";

        var result = task.RetitleStep(stepId, newTitle);

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(newTitle, task.Steps.First().Title);
    }

    /// <summary>
    /// Tests retitle step failure when step does not exist.
    /// </summary>
    /// <remarks>
    /// Verifies that retitling a non-existent step returns NotFound error.
    /// </remarks>
    [Fact(DisplayName = "WorkTask - RetitleStep Error NotFound")]
    public void WorkTask_RetitleStep_Error_NotFound()
    {
        var task = _workTaskTestFixture.GenerateWorkTask();

        var result = task.RetitleStep(Guid.NewGuid(), "New Title");

        Assert.NotNull(result);
        Assert.True(result.IsFailure);
        Assert.Equal(StepErrors.NotFound.Code, result.Error.Code);
        Assert.Equal(StepErrors.NotFound.Description, result.Error.Description);
    }

    /// <summary>
    /// Tests retitle step failure when new title is empty.
    /// </summary>
    /// <remarks>
    /// Verifies that a step cannot be retitled to an empty string.
    /// </remarks>
    [Fact(DisplayName = "WorkTask - RetitleStep Error TitleRequired")]
    public void WorkTask_RetitleStep_Error_TitleRequired()
    {
        var task = _workTaskTestFixture.GenerateWorkTask();
        task.AddStep("Step 1", Guid.NewGuid());
        var stepId = task.Steps.First().Id;

        var result = task.RetitleStep(stepId, string.Empty);

        Assert.NotNull(result);
        Assert.True(result.IsFailure);
        Assert.Equal(StepErrors.TitleRequired.Code, result.Error.Code);
        Assert.Equal(StepErrors.TitleRequired.Description, result.Error.Description);
    }

    #endregion

    #region Comment Tests

    /// <summary>
    /// Tests successful addition of a comment to a task.
    /// </summary>
    /// <remarks>
    /// Verifies that a comment can be added to a task with valid content and author.
    /// </remarks>
    [Fact(DisplayName = "WorkTask - AddComment Success")]
    public void WorkTask_AddComment_Success()
    {
        var task = _workTaskTestFixture.GenerateWorkTask();
        var authorId = Guid.NewGuid();
        var content = "This is a comment";

        var result = task.AddComment(content, authorId);

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Single(task.Comments);
        Assert.Equal(content, task.Comments.First().Content);
        Assert.Equal(authorId, task.Comments.First().AuthorId);
    }

    /// <summary>
    /// Tests adding comment failure when content is empty.
    /// </summary>
    /// <remarks>
    /// Verifies that a comment cannot be added without valid content.
    /// </remarks>
    [Fact(DisplayName = "WorkTask - AddComment Error ContentRequired")]
    public void WorkTask_AddComment_Error_ContentRequired()
    {
        var task = _workTaskTestFixture.GenerateWorkTask();

        var result = task.AddComment(string.Empty, Guid.NewGuid());

        Assert.NotNull(result);
        Assert.True(result.IsFailure);
        Assert.Equal(TaskCommentErrors.ContentRequired.Code, result.Error.Code);
        Assert.Equal(TaskCommentErrors.ContentRequired.Description, result.Error.Description);
    }

    /// <summary>
    /// Tests successful update of a comment.
    /// </summary>
    /// <remarks>
    /// Verifies that a comment can be updated with new content.
    /// </remarks>
    [Fact(DisplayName = "WorkTask - UpdateComment Success")]
    public void WorkTask_UpdateComment_Success()
    {
        var task = _workTaskTestFixture.GenerateWorkTask();
        task.AddComment("Original comment", Guid.NewGuid());
        var commentId = task.Comments.First().Id;
        var newContent = "Updated comment";

        var result = task.UpdateComment(commentId, newContent);

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(newContent, task.Comments.First().Content);
    }

    /// <summary>
    /// Tests update comment failure when comment does not exist.
    /// </summary>
    /// <remarks>
    /// Verifies that updating a non-existent comment returns NotFound error.
    /// </remarks>
    [Fact(DisplayName = "WorkTask - UpdateComment Error NotFound")]
    public void WorkTask_UpdateComment_Error_NotFound()
    {
        var task = _workTaskTestFixture.GenerateWorkTask();

        var result = task.UpdateComment(Guid.NewGuid(), "New content");

        Assert.NotNull(result);
        Assert.True(result.IsFailure);
        Assert.Equal(TaskCommentErrors.NotFound.Code, result.Error.Code);
        Assert.Equal(TaskCommentErrors.NotFound.Description, result.Error.Description);
    }

    /// <summary>
    /// Tests update comment failure when new content is empty.
    /// </summary>
    /// <remarks>
    /// Verifies that a comment cannot be updated to empty content.
    /// </remarks>
    [Fact(DisplayName = "WorkTask - UpdateComment Error ContentRequired")]
    public void WorkTask_UpdateComment_Error_ContentRequired()
    {
        var task = _workTaskTestFixture.GenerateWorkTask();
        task.AddComment("Original comment", Guid.NewGuid());
        var commentId = task.Comments.First().Id;

        var result = task.UpdateComment(commentId, string.Empty);

        Assert.NotNull(result);
        Assert.True(result.IsFailure);
        Assert.Equal(TaskCommentErrors.ContentRequired.Code, result.Error.Code);
        Assert.Equal(TaskCommentErrors.ContentRequired.Description, result.Error.Description);
    }

    /// <summary>
    /// Tests successful removal of a comment.
    /// </summary>
    /// <remarks>
    /// Verifies that a comment can be removed from a task.
    /// </remarks>
    [Fact(DisplayName = "WorkTask - RemoveComment Success")]
    public void WorkTask_RemoveComment_Success()
    {
        var task = _workTaskTestFixture.GenerateWorkTask();
        task.AddComment("Comment to remove", Guid.NewGuid());
        var commentId = task.Comments.First().Id;

        var result = task.RemoveComment(commentId, "Comment to remove");

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Empty(task.Comments);
    }

    /// <summary>
    /// Tests remove comment failure when comment does not exist.
    /// </summary>
    /// <remarks>
    /// Verifies that removing a non-existent comment returns NotFound error.
    /// </remarks>
    [Fact(DisplayName = "WorkTask - RemoveComment Error NotFound")]
    public void WorkTask_RemoveComment_Error_NotFound()
    {
        var task = _workTaskTestFixture.GenerateWorkTask();

        var result = task.RemoveComment(Guid.NewGuid(), "Some content");

        Assert.NotNull(result);
        Assert.True(result.IsFailure);
        Assert.Equal(TaskCommentErrors.NotFound.Code, result.Error.Code);
        Assert.Equal(TaskCommentErrors.NotFound.Description, result.Error.Description);
    }

    #endregion

    #region Utility Tests

    /// <summary>
    /// Tests the TotalStepHours property.
    /// </summary>
    /// <remarks>
    /// Verifies that the total step hours is correctly calculated as the sum of all step hours.
    /// </remarks>
    [Fact(DisplayName = "WorkTask - TotalStepHours Calculated Correctly")]
    public void WorkTask_TotalStepHours_CalculatedCorrectly()
    {
        var task = _workTaskTestFixture.GenerateWorkTask();
        var stateId = task.CurrentStateId;

        // Add multiple steps and mark them as done
        task.AddStep("Step 1", stateId);
        task.AddStep("Step 2", stateId);

        var step1Id = task.Steps.ElementAt(0).Id;
        var step2Id = task.Steps.ElementAt(1).Id;

        task.SetStepStatusDone(step1Id, 3);
        task.SetStepStatusDone(step2Id, 2);

        Assert.Equal(5, task.TotalStepHours);
        Assert.Equal(5, task.ActualHours);
    }

    #endregion
}
