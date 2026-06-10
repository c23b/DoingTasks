using DoingTasks.Domain.Auditing;
using DoingTasks.Domain.Tasks;
using DoingTasks.Domain.Workspaces;
using DoingTasks.Infrastructure.Persistence.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DoingTasks.Infrastructure.Persistence.Configurations;

public sealed class WorkspaceAuditLogConfiguration : EntityConfiguration<WorkspaceAuditLog>
{
    public override void Configure(EntityTypeBuilder<WorkspaceAuditLog> builder)
    {
        base.Configure(builder);

        builder.ToTable("workspace_audit_logs");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .IsRequired()
            .HasColumnName("id")
            .HasColumnType("uuid");

        builder.Property(a => a.WorkspaceId)
            .IsRequired()
            .HasColumnName("workspace_id")
            .HasColumnType("uuid");

        builder.Property(a => a.TaskId)
            .HasColumnName("task_id")
            .HasColumnType("uuid");

        builder.Property(a => a.ActorId)
            .IsRequired()
            .HasColumnName("actor_id")
            .HasColumnType("uuid");

        builder.Property(a => a.ActorNickname)
            .IsRequired()
            .HasColumnName("actor_nickname")
            .HasColumnType("varchar(30)");

        builder.Property(a => a.Action)
            .IsRequired()
            .HasColumnName("action")
            .HasColumnType("integer");

        builder.Property(a => a.Payload)
            .IsRequired()
            .HasColumnName("payload")
            .HasColumnType("jsonb");

        builder.Property(a => a.OccurredAt)
            .IsRequired()
            .HasColumnName("occurred_at")
            .HasColumnType("timestamp with time zone");

        builder.HasOne<Workspace>()
               .WithMany()
               .HasForeignKey(a => a.WorkspaceId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<WorkTask>()
               .WithMany()
               .HasForeignKey(a => a.TaskId)
               .OnDelete(DeleteBehavior.Restrict);

        // Índices para queries de auditoria
        builder.HasIndex(a => a.WorkspaceId)
            .HasDatabaseName("ix_workspace_audit_logs_workspace_id");

        builder.HasIndex(a => a.TaskId)
            .HasDatabaseName("ix_workspace_audit_logs_task_id");

        builder.HasIndex(a => a.ActorId)
            .HasDatabaseName("ix_workspace_audit_logs_actor_id");

        builder.HasIndex(a => a.OccurredAt)
            .HasDatabaseName("ix_workspace_audit_logs_occurred_at");
    }
}
