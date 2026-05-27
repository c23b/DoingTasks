namespace DoingTasks.Domain.Auditing;

public enum AuditAction
{
    UserCreated,
    UserUpdated,
    WorkspaceCreated,
    WorkspaceRenamed,
    WorkspaceCollaboratorEditingToggled,
    WorkspaceRegrouped,
    WorkspaceStateReordered,
    WorkspaceStateAdded,
    WorkspaceStateRenamed,
    MemberInvited,
    MemberRoleChanged,
    TaskCreated,
    TaskStateTransitioned,
    TaskBlocked,
    TaskUnblocked,
    StepCompleted
}