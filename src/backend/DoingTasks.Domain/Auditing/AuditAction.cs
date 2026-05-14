namespace DoingTasks.Domain.Auditing;

public enum AuditAction
{
    TaskCreated,
    TaskStateTransitioned,
    TaskBlocked,
    TaskUnblocked,
    StepCompleted,
    MemberInvited,
    WorkspaceStateAdded
}