using DoingTasks.Domain.Tasks;
using DoingTasks.Domain.Users;
using DoingTasks.Infrastructure.Persistence.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DoingTasks.Infrastructure.Persistence.Configurations;

public sealed class TaskCommentConfiguration : EntityConfiguration<TaskComment>
{
    public override void Configure(EntityTypeBuilder<TaskComment> builder)
    {
        base.Configure(builder);

        builder.ToTable("task_comments");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .IsRequired()
            .HasColumnName("id")
            .HasColumnType("uuid");

        builder.Property(c => c.WorkTaskId)
            .IsRequired()
            .HasColumnName("work_task_id")
            .HasColumnType("uuid");

        builder.Property(c => c.AuthorId)
            .IsRequired()
            .HasColumnName("author_id")
            .HasColumnType("uuid");

        builder.Property(c => c.Content)
            .IsRequired()
            .HasColumnName("content")
            .HasColumnType("varchar(2000)");

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(c => c.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
