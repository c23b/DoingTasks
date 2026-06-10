using DoingTasks.Domain.Users;
using DoingTasks.Infrastructure.Persistence.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DoingTasks.Infrastructure.Persistence.Configurations;

public sealed class UserConfiguration : EntityConfiguration<User>
{
    public override void Configure(EntityTypeBuilder<User> builder)
    {
        base.Configure(builder);

        builder.ToTable("users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
               .IsRequired()
               .HasColumnName("id")
               .HasColumnType("uuid");

        builder.Property(u => u.FullName)
               .IsRequired()
               .HasColumnName("full_name")
               .HasColumnType("varchar(200)");

        builder.Property(u => u.Email)
               .IsRequired()
               .HasColumnName("email")
               .HasColumnType("varchar(200)");

        builder.HasIndex(u => u.Email)
               .IsUnique();

        builder.Property(u => u.IdentityId)
               .IsRequired()
               .HasColumnName("identity_id")
               .HasColumnType("varchar(100)");

        builder.HasIndex(u => u.IdentityId)
               .IsUnique();

        builder.Property(u => u.BirthDate)
               .IsRequired()
               .HasColumnName("birth_date")
               .HasColumnType("date");

        builder.OwnsOne(u => u.Nickname, nickname =>
        {
            nickname.Property(n => n.Value)
                    .IsRequired()
                    .HasColumnName("nickname")
                    .HasColumnType("varchar(30)");
        });
    }
}
