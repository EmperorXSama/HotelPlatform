using HotelPlatform.Domain.ReferenceData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelPlatform.Infrastructure.Data.Configurations;

public class HotelAmenityDefinitionConfiguration : IEntityTypeConfiguration<HotelAmenityDefinition>
{
    public void Configure(EntityTypeBuilder<HotelAmenityDefinition> builder)
    {
        builder.ToTable("hotel_amenity_definitions");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .HasColumnName("id")
            .HasStronglyTypedId()
            .ValueGeneratedNever();

        builder.Property(a => a.Code)
            .HasColumnName("code")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(a => a.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(a => a.Icon)
            .HasColumnName("icon")
            .HasMaxLength(500);

        builder.Property(a => a.Category)
            .HasColumnName("category")
            .HasMaxLength(100);

        builder.Property(a => a.IsActive)
            .HasColumnName("is_active")
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(a => a.IsSystemDefined)
            .HasColumnName("is_system_defined")
            .IsRequired()
            .HasDefaultValue(false);

        // Indexes
        builder.HasIndex(a => a.Code)
            .IsUnique();

        builder.HasIndex(a => a.IsActive);
    }
}