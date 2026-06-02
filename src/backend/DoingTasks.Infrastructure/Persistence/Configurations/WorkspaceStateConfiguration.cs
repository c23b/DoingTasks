using DoingTasks.Domain.Workspaces;
using DoingTasks.Infrastructure.Persistence.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DoingTasks.Infrastructure.Persistence.Configurations;

public sealed class WorkspaceStateConfiguration : EntityConfiguration<WorkspaceState>
{
    public override void Configure(EntityTypeBuilder<WorkspaceState> builder)
    {
        base.Configure(builder);

        builder.ToTable("workspace_states");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .IsRequired()
            .HasColumnName("id")
            .HasColumnType("uuid");

        builder.Property(s => s.WorkspaceId)
            .IsRequired()
            .HasColumnName("workspace_id")
            .HasColumnType("uuid");

        builder.Property(s => s.Name)
            .IsRequired()
            .HasColumnName("name")
            .HasColumnType("varchar(200)");

        builder.OwnsOne(s => s.Order, order =>
        {
            order.Property(o => o.Value)
                .IsRequired()
                .HasColumnName("order")
                .HasColumnType("integer");
        });

    }
}
