using HotelPlatform.Domain.Common.ValueObjects;
using HotelPlatform.Domain.Hotels.Entities;
using HotelPlatform.Infrastructure.Data.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace HotelPlatform.Infrastructure.Data.Configurations;

public class RoomConfiguration : IEntityTypeConfiguration<Room>
{
    public void Configure(EntityTypeBuilder<Room> builder)
    {
        builder.ToTable("Rooms");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .HasColumnName("id")
            .HasStronglyTypedId()
            .ValueGeneratedNever();

        builder.Property(r => r.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(r => r.RoomTypeId)
            .HasColumnName("room_type_id")
            .HasStronglyTypedId()
            .IsRequired();

        builder.Property(r => r.Capacity)
            .HasColumnName("capacity")
            .IsRequired();

        builder.Property(r => r.Description)
            .HasColumnName("description")
            .HasMaxLength(2000);

        // BasePrice as owned type
        builder.OwnsOne(r => r.BasePrice, priceBuilder =>
        {
            priceBuilder.Property(m => m.Amount)
                .HasColumnName("base_price_amount")
                .HasPrecision(18, 2)
                .IsRequired();

            priceBuilder.Property(m => m.Currency)
                .HasColumnName("base_price_currency")
                .HasConversion(new CurrencyConverter())
                .HasMaxLength(3)
                .IsRequired();
        });

        // Pictures as owned collection
        builder.OwnsMany(r => r.Pictures, pictureBuilder =>
        {
            pictureBuilder.ToTable("room_pictures");

            pictureBuilder.WithOwner()
                .HasForeignKey("room_id");

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

            pictureBuilder.HasIndex("room_id", nameof(RoomPicture.StoredFileId))
                .IsUnique();
        });

        // Amenities as owned collection
        builder.OwnsMany(r => r.Amenities, amenityBuilder =>
        {
            amenityBuilder.ToTable("room_selected_amenities");
            amenityBuilder.WithOwner().HasForeignKey("room_id");
            

            amenityBuilder.Property<int>("id")
                .ValueGeneratedOnAdd();

            amenityBuilder.HasKey("id");

            amenityBuilder.Property(a => a.AmenityDefinitionId)
                .HasStronglyTypedId()
                .HasColumnName("amenity_definition_id")
                .IsRequired();

            // Upcharge as nested owned type
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

            amenityBuilder.HasIndex("room_id", nameof(RoomSelectedAmenity.AmenityDefinitionId))
                .IsUnique();
        });

        // Index
        builder.HasIndex(r => r.RoomTypeId);
    }
}