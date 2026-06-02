using DoingTasks.Domain.Users;
using DoingTasks.Domain.Workspaces;
using DoingTasks.Infrastructure.Persistence.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DoingTasks.Infrastructure.Persistence.Configurations;

public sealed class WorkspaceMemberConfiguration : EntityConfiguration<WorkspaceMember>
{
    public override void Configure(EntityTypeBuilder<WorkspaceMember> builder)
    {
        base.Configure(builder);

        builder.ToTable("workspace_members");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Id)
            .IsRequired()
            .HasColumnName("id")
            .HasColumnType("uuid");

        builder.Property(m => m.WorkspaceId)
            .IsRequired()
            .HasColumnName("workspace_id")
            .HasColumnType("uuid");

        builder.Property(m => m.UserId)
            .IsRequired()
            .HasColumnName("user_id")
            .HasColumnType("uuid");

        builder.Property(m => m.Role)
            .IsRequired()
            .HasColumnName("role")
            .HasColumnType("integer");

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(m => m.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(m => new { m.WorkspaceId, m.UserId })
            .IsUnique()
            .HasDatabaseName("ix_workspace_members_workspace_id_user_id");
    }
}
