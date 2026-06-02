using DoingTasks.Domain.Tasks;
using DoingTasks.Domain.Users;
using DoingTasks.Domain.Workspaces;
using DoingTasks.Infrastructure.Persistence.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DoingTasks.Infrastructure.Persistence.Configurations;

public sealed class WorkTaskConfiguration : EntityConfiguration<WorkTask>
{
    public override void Configure(EntityTypeBuilder<WorkTask> builder)
    {
        base.Configure(builder);

        builder.ToTable("work_tasks");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .IsRequired()
            .HasColumnName("id")
            .HasColumnType("uuid");

        builder.Property(t => t.Title)
            .IsRequired()
            .HasColumnName("title")
            .HasColumnType("varchar(200)");

        builder.Property(t => t.Description)
            .HasColumnName("description")
            .HasColumnType("varchar(2000)");

        builder.Property(t => t.WorkspaceId)
            .IsRequired()
            .HasColumnName("workspace_id")
            .HasColumnType("uuid");

        builder.Property(t => t.CurrentStateId)
            .IsRequired()
            .HasColumnName("current_state_id")
            .HasColumnType("uuid");

        builder.Property(t => t.PlannedHours)
            .HasColumnName("planned_hours");

        builder.Property(t => t.ActualHours)
            .IsRequired()
            .HasColumnName("actual_hours")
            .HasDefaultValue(0);

        builder.Property(t => t.IsBlocked)
            .IsRequired()
            .HasColumnName("is_blocked")
            .HasDefaultValue(false);

        builder.Property(t => t.BlockJustification)
            .HasColumnName("block_justification")
            .HasColumnType("varchar(500)");

        builder.Property(t => t.AssignedUserId)
            .HasColumnName("assigned_user_id")
            .HasColumnType("uuid");

        builder.OwnsOne(t => t.Complexity, complexity =>
        {
            complexity.Property(c => c.Value)
                .HasColumnName("complexity")
                .HasColumnType("integer");
        });

        builder.HasMany(t => t.Steps)
            .WithOne()
            .HasForeignKey("work_task_id")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(t => t.Comments)
            .WithOne()
            .HasForeignKey("work_task_id")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Workspace>()
            .WithMany()
            .HasForeignKey(t => t.WorkspaceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(t => t.AssignedUserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Ignore(t => t.TotalStepHours);
    }
}