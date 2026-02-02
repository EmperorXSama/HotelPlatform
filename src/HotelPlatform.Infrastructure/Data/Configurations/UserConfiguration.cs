
using HotelPlatform.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelPlatform.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .HasStronglyTypedId()
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(u => u.IdentityId)
            .HasMaxLength(256)
            .HasColumnName("identity_id")
            .IsRequired();

        builder.Property(u => u.Email)
            .HasMaxLength(256)
            .HasColumnName("email")
            .IsRequired();

        builder.Property(u => u.DisplayName)
            .HasMaxLength(200)
            .HasColumnName("display_name")
            .IsRequired();

        builder.Property(u => u.CreatedAt)
            .HasColumnName("created")
            .IsRequired();

        builder.Property(u => u.LastModified)
            .HasColumnName("last_modified");

        // Indexes
        builder.HasIndex(u => u.IdentityId)
            .IsUnique();

        builder.HasIndex(u => u.Email)
            .IsUnique();
    }
}