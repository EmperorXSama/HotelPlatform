// Infrastructure/Data/Configurations/RatingConfiguration.cs
using HotelPlatform.Domain.Ratings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelPlatform.Infrastructure.Data.Configurations;

public class RatingConfiguration : IEntityTypeConfiguration<Rating>
{
    public void Configure(EntityTypeBuilder<Rating> builder)
    {
        builder.ToTable("ratings");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .HasStronglyTypedId()
            .ValueGeneratedNever();

        builder.Property(r => r.HotelId)
            .HasStronglyTypedId()
            .IsRequired();

        builder.Property(r => r.UserId)
            .HasStronglyTypedId()
            .IsRequired();

        builder.Property(r => r.Score)
            .IsRequired();

        builder.Property(r => r.Comment)
            .HasMaxLength(2000);

        builder.Property(r => r.CreatedAt)
            .IsRequired();

        builder.Property(r => r.LastModified);

        // Indexes
        builder.HasIndex(r => r.HotelId);

        builder.HasIndex(r => r.UserId);

        // Unique constraint: one rating per user per hotel
        builder.HasIndex(r => new { r.HotelId, r.UserId })
            .IsUnique();
    }
}