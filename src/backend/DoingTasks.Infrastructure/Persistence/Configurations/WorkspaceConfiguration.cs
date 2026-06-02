using DoingTasks.Domain.Users;
using DoingTasks.Domain.Workspaces;
using DoingTasks.Infrastructure.Persistence.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DoingTasks.Infrastructure.Persistence.Configurations;

public sealed class WorkspaceConfiguration : EntityConfiguration<Workspace>
{
    public override void Configure(EntityTypeBuilder<Workspace> builder)
    {
        base.Configure(builder);

        builder.ToTable("workspaces");

        builder.HasKey(w => w.Id);

        builder.Property(w => w.Id)
               .IsRequired()
               .HasColumnName("id")
               .HasColumnType("uuid");

        builder.Property(w => w.Name)
               .IsRequired()
               .HasColumnName("name")
               .HasColumnType("varchar(200)");

        builder.Property(w => w.GroupName)
               .HasColumnName("group_name")
               .HasColumnType("varchar(200)");

        builder.Property(w => w.OwnerId)
               .IsRequired()
               .HasColumnName("owner_id")
               .HasColumnType("uuid");

        builder.HasOne<User>()
               .WithMany()
               .HasForeignKey(w => w.OwnerId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.Property(w => w.AllowCollaboratorEditing)
               .IsRequired()
               .HasColumnName("allow_collaborator_editing")
               .HasColumnType("boolean")
               .HasDefaultValue(false);

        builder.HasMany(w => w.States)
               .WithOne()
               .HasForeignKey("workspace_id")
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(w => w.Members)
               .WithOne()
               .HasForeignKey("workspace_id")
               .OnDelete(DeleteBehavior.Cascade);

        // Ignorar propriedades computadas — não persistidas
        builder.Ignore(w => w.IsOperational);
        builder.Ignore(w => w.InitialState);
    }
}
