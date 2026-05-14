using DoingTasks.SharedKernel.Domain;
using System.Text.Json;

namespace DoingTasks.Domain.Auditing;

// ─── AUDIT LOG ───────────────────────────────────────────

public sealed class WorkspaceAuditLog : Entity
{
    public Guid WorkspaceId { get; private set; }
    public Guid? TaskId { get; private set; }
    public Guid ActorId { get; private set; }
    public string ActorNickname { get; private set; }
    public AuditAction Action { get; private set; }
    public string Payload { get; private set; }
    public DateTime OccurredAt { get; private set; }

    private WorkspaceAuditLog() { }

    public static WorkspaceAuditLog Create(
        Guid workspaceId,
        Guid? taskId,
        Guid actorId,
        string actorNickname,
        AuditAction action,
        object payload) =>
        new()
        {
            Id = Guid.NewGuid(),
            WorkspaceId = workspaceId,
            TaskId = taskId,
            ActorId = actorId,
            ActorNickname = actorNickname,
            Action = action,
            Payload = JsonSerializer.Serialize(payload),
            OccurredAt = DateTime.UtcNow
        };
}
