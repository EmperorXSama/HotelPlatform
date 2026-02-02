using HotelPlatform.Domain.Files;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelPlatform.Infrastructure.Data.Configurations;

public class StoredFileConfiguration : IEntityTypeConfiguration<StoredFile>
{
    public void Configure(EntityTypeBuilder<StoredFile> builder)
    {
        builder.ToTable("stored_files");

        builder.HasKey(f => f.Id);

        builder.Property(f => f.Id)
            .HasStronglyTypedId()
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(f => f.OwnerId)
            .HasStronglyTypedId()
            .HasColumnName("owner_id")
            .IsRequired();

        builder.Property(f => f.OriginalFileName)
            .HasMaxLength(500)
            .HasColumnName("original_file_name")
            .IsRequired();

        builder.Property(f => f.StoredFileName)
            .HasMaxLength(500)
            .HasColumnName("stored_file_name")
            .IsRequired();

        builder.Property(f => f.ContentType)
            .HasMaxLength(100)
            .HasColumnName("content_type")
            .IsRequired();

        builder.Property(f => f.SizeInBytes)
            .HasColumnName("size_in_bytes")
            .IsRequired();

        builder.Property(f => f.Url)
            .HasColumnName("url")
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(f => f.CreatedAt)
            .HasColumnName("created")
            .IsRequired();

        // Indexes
        builder.HasIndex(f => f.OwnerId);

        builder.HasIndex(f => f.StoredFileName)
            .IsUnique();
    }
}