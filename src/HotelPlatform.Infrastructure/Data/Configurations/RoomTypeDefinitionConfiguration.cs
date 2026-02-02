using HotelPlatform.Domain.ReferenceData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelPlatform.Infrastructure.Data.Configurations;

public class RoomTypeDefinitionConfiguration : IEntityTypeConfiguration<RoomTypeDefinition>
{
    public void Configure(EntityTypeBuilder<RoomTypeDefinition> builder)
    {
        builder.ToTable("room_type_definitions");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .HasColumnName("id")
            .HasStronglyTypedId()
            .ValueGeneratedNever();

        builder.Property(r => r.Code)
            .HasColumnName("code")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(r => r.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(r => r.Description)
            .HasColumnName("description")
            .HasMaxLength(1000);

        builder.Property(r => r.DefaultCapacity)
            .HasColumnName("default_capacity")
            .IsRequired();

        builder.Property(r => r.Icon)
            .HasColumnName("icon")
            .HasMaxLength(500);

        builder.Property(r => r.IsActive)
            .HasColumnName("is_active")
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(r => r.IsSystemDefined)
            .HasColumnName("is_system_defined")
            .IsRequired()
            .HasDefaultValue(false);

        // Indexes
        builder.HasIndex(r => r.Code)
            .IsUnique();

        builder.HasIndex(r => r.IsActive);
    }
}