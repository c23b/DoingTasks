namespace DoingTasks.Domain.Auditing;

public enum AuditAction
{
    TaskCreated,
    TaskStateTransitioned,
    TaskBlocked,
    TaskUnblocked,
    StepCompleted,
    MemberInvited,
    WorkspaceStateAdded,
    WorkspaceRenamed,
    WorkspaceRegrouped,
    CollaboratorEditingToggled,
    WorkspaceStateReordered,
    WorkspaceStateRenamed,
    MemberRoleChanged
}