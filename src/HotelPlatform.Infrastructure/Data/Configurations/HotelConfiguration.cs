using HotelPlatform.Domain.Common.ValueObjects;
using HotelPlatform.Domain.Enums;
using HotelPlatform.Domain.Hotels;
using HotelPlatform.Domain.Hotels.Entities;
using HotelPlatform.Infrastructure.Data.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelPlatform.Infrastructure.Data.Configurations;

public class HotelConfiguration : IEntityTypeConfiguration<Hotel>
{
    public void Configure(EntityTypeBuilder<Hotel> builder)
    {
        builder.ToTable("Hotels");

        builder.HasKey(h => h.Id);

        builder.Property(h => h.Id)
            .HasColumnName("id")
            .HasStronglyTypedId()
            .ValueGeneratedNever();

        builder.Property(h => h.OwnerId)
            .HasColumnName("owner_id")
            .HasStronglyTypedId()
            .IsRequired();

        builder.Property(h => h.Name)
            .HasColumnName("name")
            .HasMaxLength(Hotel.NameMaxLength)
            .IsRequired();

        builder.Property(h => h.Description)
            .HasColumnName("description")
            .HasMaxLength(Hotel.DescriptionMaxLength);

        builder.Property(h => h.Status)
            .HasColumnName("status")
            .IsRequired()
            .HasDefaultValue(HotelStatus.Draft);

        builder.Property(h => h.CreatedAt)
            .HasColumnName("created")
            .IsRequired();

        builder.Property(h => h.LastModified)
            .HasColumnName("last_modified");

        // Address as owned type
        builder.OwnsOne(h => h.Address, addressBuilder =>
        {
            addressBuilder.Property(a => a.Street)
                .HasColumnName("address_street")
                .HasMaxLength(500)
                .IsRequired();

            addressBuilder.Property(a => a.City)
                .HasColumnName("address_city")
                .HasMaxLength(200)
                .IsRequired();

            addressBuilder.Property(a => a.Country)
                .HasColumnName("address_country")
                .HasMaxLength(100)
                .IsRequired();

            addressBuilder.Property(a => a.PostalCode)
                .HasColumnName("address_postal_code")
                .HasMaxLength(20);

            // Coordinates as nested owned type
            addressBuilder.OwnsOne(a => a.Coordinates, coordBuilder =>
            {
                coordBuilder.Property(c => c.Latitude)
                    .HasColumnName("address_latitude");

                coordBuilder.Property(c => c.Longitude)
                    .HasColumnName("address_longitude");
            });
        });

        // AggregatedRating as owned type
        builder.OwnsOne(h => h.AggregatedRating, ratingBuilder =>
        {
            ratingBuilder.Property(r => r.AverageScore)
                .HasColumnName("rating_average")
                .HasPrecision(3, 2)
                .HasDefaultValue(0);

            ratingBuilder.Property(r => r.TotalCounts)
                .HasColumnName("rating_total_counts")
                .HasDefaultValue(0);
        });

        // Pictures as owned collection
        builder.OwnsMany(h => h.Pictures, pictureBuilder =>
        {
            pictureBuilder.ToTable("hotel_pictures");

            pictureBuilder.WithOwner()
                .HasForeignKey("hotel_id");

            pictureBuilder.Property<int>("id")
                .ValueGeneratedOnAdd();

            pictureBuilder.HasKey("id");

            pictureBuilder.Property(p => p.StoredFileId)
                .HasColumnName("stored_file_id")
                .HasStronglyTypedId()
                .IsRequired();

            pictureBuilder.Property(p => p.AltText)
                .HasColumnName("alt_text")
                .HasMaxLength(500);

            pictureBuilder.Property(p => p.IsMain)
                .HasColumnName("is_main")
                .IsRequired();

            pictureBuilder.Property(p => p.DisplayOrder)
                .HasColumnName("display_order")
                .IsRequired();

            pictureBuilder.HasIndex("hotel_id", nameof(HotelPicture.StoredFileId))
                .IsUnique();
        });

        // Amenities as owned collection
        builder.OwnsMany(h => h.Amenities, amenityBuilder =>
        {
            amenityBuilder.ToTable("hotel_selected_amenities");

            amenityBuilder.WithOwner()
                .HasForeignKey("hotel_id");

            // Remove the shadow id property
            // Use composite key instead
            amenityBuilder.HasKey("hotel_id", nameof(HotelSelectedAmenity.AmenityDefinitionId));

            amenityBuilder.Property(a => a.AmenityDefinitionId)
                .HasColumnName("amenity_definition_id")
                .HasStronglyTypedId()
                .IsRequired();

            amenityBuilder.OwnsOne(a => a.Upcharge, upchargeBuilder =>
            {
                upchargeBuilder.Property(u => u.Type)
                    .HasColumnName("upcharge_type")
                    .IsRequired();

                upchargeBuilder.Property(u => u.Amount)
                    .HasColumnName("upcharge_amount")
                    .HasPrecision(18, 4)
                    .IsRequired();

                upchargeBuilder.Property(u => u.Currency)
                    .HasColumnName("upcharge_currency")
                    .HasConversion(new CurrencyConverter())
                    .HasMaxLength(3);
            });
        });
        // Rooms - one-to-many relationship
        builder.HasMany(h => h.Rooms)
            .WithOne()
            .HasForeignKey("hotel_id")
            .OnDelete(DeleteBehavior.Cascade);

        // Navigation property for rooms needs backing field
        builder.Navigation(h => h.Rooms)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        // Indexes
        builder.HasIndex(h => h.OwnerId);
        builder.HasIndex(h => h.Status);
        builder.HasIndex(h => h.Name);
        
    }
}