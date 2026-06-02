using DoingTasks.Domain.Tasks;
using DoingTasks.Domain.Users;
using DoingTasks.Infrastructure.Persistence.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DoingTasks.Infrastructure.Persistence.Configurations;

// Infrastructure/Persistence/Configurations/StepConfiguration.cs
public sealed class StepConfiguration : EntityConfiguration<Step>
{
    public override void Configure(EntityTypeBuilder<Step> builder)
    {
        base.Configure(builder);

        builder.ToTable("steps");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .IsRequired()
            .HasColumnName("id")
            .HasColumnType("uuid");

        builder.Property(s => s.WorkTaskId)
            .IsRequired()
            .HasColumnName("work_task_id")
            .HasColumnType("uuid");

        builder.Property(s => s.Title)
            .IsRequired()
            .HasColumnName("title")
            .HasColumnType("varchar(200)");

        builder.Property(s => s.StepStatus)
            .IsRequired()
            .HasColumnName("status")
            .HasColumnType("integer")
            .HasDefaultValue(StepStatus.Pending);

        builder.Property(s => s.CompletedAt)
            .HasColumnName("completed_at")
            .HasColumnType("timestamp with time zone");

        builder.Property(s => s.ActualHours)
            .IsRequired()
            .HasColumnName("actual_hours")
            .HasDefaultValue(0);

        builder.Property(s => s.AssignedUserId)
            .HasColumnName("assigned_user_id")
            .HasColumnType("uuid");

        builder.HasOne<WorkTask>()
            .WithMany(t => t.Steps)
            .HasForeignKey(s => s.WorkTaskId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(s => s.AssignedUserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}