// Infrastructure/Data/Migrations/SeedReferenceData.cs
using HotelPlatform.Domain.Common.StronglyTypedIds;
using HotelPlatform.Domain.ReferenceData;
using Microsoft.EntityFrameworkCore;

namespace HotelPlatform.Infrastructure.Data.Migrations;

public static class SeedReferenceData
{
    public static void SeedData(ModelBuilder builder)
    {
        SeedHotelAmenities(builder);
        SeedRoomAmenities(builder);
        SeedRoomTypes(builder);
    }

    private static void SeedHotelAmenities(ModelBuilder builder)
    {
        var amenities = new List<object>
        {
            // Recreation
            new
            {
                Id = new HotelAmenityDefinitionId(Guid.Parse("10000000-0000-0000-0000-000000000001")),
                Code = "POOL",
                Name = "Swimming Pool",
                Icon = "pool",
                Category = "Recreation",
                IsActive = true,
                IsSystemDefined = true
            },
            new
            {
                Id = new HotelAmenityDefinitionId(Guid.Parse("10000000-0000-0000-0000-000000000002")),
                Code = "GYM",
                Name = "Fitness Center",
                Icon = "gym",
                Category = "Recreation",
                IsActive = true,
                IsSystemDefined = true
            },
            new
            {
                Id = new HotelAmenityDefinitionId(Guid.Parse("10000000-0000-0000-0000-000000000003")),
                Code = "SPA",
                Name = "Spa & Wellness",
                Icon = "spa",
                Category = "Recreation",
                IsActive = true,
                IsSystemDefined = true
            },

            // Services
            new
            {
                Id = new HotelAmenityDefinitionId(Guid.Parse("10000000-0000-0000-0000-000000000004")),
                Code = "RESTAURANT",
                Name = "Restaurant",
                Icon = "restaurant",
                Category = "Services",
                IsActive = true,
                IsSystemDefined = true
            },
            new
            {
                Id = new HotelAmenityDefinitionId(Guid.Parse("10000000-0000-0000-0000-000000000005")),
                Code = "BAR",
                Name = "Bar & Lounge",
                Icon = "bar",
                Category = "Services",
                IsActive = true,
                IsSystemDefined = true
            },
            new
            {
                Id = new HotelAmenityDefinitionId(Guid.Parse("10000000-0000-0000-0000-000000000006")),
                Code = "ROOM_SERVICE",
                Name = "Room Service",
                Icon = "room-service",
                Category = "Services",
                IsActive = true,
                IsSystemDefined = true
            },
            new
            {
                Id = new HotelAmenityDefinitionId(Guid.Parse("10000000-0000-0000-0000-000000000007")),
                Code = "CONCIERGE",
                Name = "Concierge Service",
                Icon = "concierge",
                Category = "Services",
                IsActive = true,
                IsSystemDefined = true
            },
            new
            {
                Id = new HotelAmenityDefinitionId(Guid.Parse("10000000-0000-0000-0000-000000000008")),
                Code = "LAUNDRY",
                Name = "Laundry Service",
                Icon = "laundry",
                Category = "Services",
                IsActive = true,
                IsSystemDefined = true
            },

            // Facilities
            new
            {
                Id = new HotelAmenityDefinitionId(Guid.Parse("10000000-0000-0000-0000-000000000009")),
                Code = "PARKING",
                Name = "Parking",
                Icon = "parking",
                Category = "Facilities",
                IsActive = true,
                IsSystemDefined = true
            },
            new
            {
                Id = new HotelAmenityDefinitionId(Guid.Parse("10000000-0000-0000-0000-00000000000A")),
                Code = "WIFI",
                Name = "Free WiFi",
                Icon = "wifi",
                Category = "Facilities",
                IsActive = true,
                IsSystemDefined = true
            },
            new
            {
                Id = new HotelAmenityDefinitionId(Guid.Parse("10000000-0000-0000-0000-00000000000B")),
                Code = "BUSINESS_CENTER",
                Name = "Business Center",
                Icon = "business",
                Category = "Facilities",
                IsActive = true,
                IsSystemDefined = true
            },
            new
            {
                Id = new HotelAmenityDefinitionId(Guid.Parse("10000000-0000-0000-0000-00000000000C")),
                Code = "MEETING_ROOMS",
                Name = "Meeting Rooms",
                Icon = "meeting",
                Category = "Facilities",
                IsActive = true,
                IsSystemDefined = true
            },

            // Accessibility
            new
            {
                Id = new HotelAmenityDefinitionId(Guid.Parse("10000000-0000-0000-0000-00000000000D")),
                Code = "WHEELCHAIR",
                Name = "Wheelchair Accessible",
                Icon = "wheelchair",
                Category = "Accessibility",
                IsActive = true,
                IsSystemDefined = true
            },
            new
            {
                Id = new HotelAmenityDefinitionId(Guid.Parse("10000000-0000-0000-0000-00000000000E")),
                Code = "ELEVATOR",
                Name = "Elevator",
                Icon = "elevator",
                Category = "Accessibility",
                IsActive = true,
                IsSystemDefined = true
            },

            // Family
            new
            {
                Id = new HotelAmenityDefinitionId(Guid.Parse("10000000-0000-0000-0000-00000000000F")),
                Code = "KIDS_CLUB",
                Name = "Kids Club",
                Icon = "kids",
                Category = "Family",
                IsActive = true,
                IsSystemDefined = true
            },
            new
            {
                Id = new HotelAmenityDefinitionId(Guid.Parse("10000000-0000-0000-0000-000000000010")),
                Code = "BABYSITTING",
                Name = "Babysitting Service",
                Icon = "babysitting",
                Category = "Family",
                IsActive = true,
                IsSystemDefined = true
            },

            // Pets
            new
            {
                Id = new HotelAmenityDefinitionId(Guid.Parse("10000000-0000-0000-0000-000000000011")),
                Code = "PET_FRIENDLY",
                Name = "Pet Friendly",
                Icon = "pet",
                Category = "Pets",
                IsActive = true,
                IsSystemDefined = true
            }
        };

        builder.Entity<HotelAmenityDefinition>().HasData(amenities);
    }

    private static void SeedRoomAmenities(ModelBuilder builder)
    {
        var amenities = new List<object>
        {
            // Comfort
            new
            {
                Id = new RoomAmenityDefinitionId(Guid.Parse("20000000-0000-0000-0000-000000000001")),
                Code = "AC",
                Name = "Air Conditioning",
                Icon = "ac",
                Category = "Comfort",
                IsActive = true,
                IsSystemDefined = true
            },
            new
            {
                Id = new RoomAmenityDefinitionId(Guid.Parse("20000000-0000-0000-0000-000000000002")),
                Code = "HEATING",
                Name = "Heating",
                Icon = "heating",
                Category = "Comfort",
                IsActive = true,
                IsSystemDefined = true
            },
            new
            {
                Id = new RoomAmenityDefinitionId(Guid.Parse("20000000-0000-0000-0000-000000000003")),
                Code = "BALCONY",
                Name = "Balcony",
                Icon = "balcony",
                Category = "Comfort",
                IsActive = true,
                IsSystemDefined = true
            },
            new
            {
                Id = new RoomAmenityDefinitionId(Guid.Parse("20000000-0000-0000-0000-000000000004")),
                Code = "SEA_VIEW",
                Name = "Sea View",
                Icon = "sea-view",
                Category = "Comfort",
                IsActive = true,
                IsSystemDefined = true
            },
            new
            {
                Id = new RoomAmenityDefinitionId(Guid.Parse("20000000-0000-0000-0000-000000000005")),
                Code = "CITY_VIEW",
                Name = "City View",
                Icon = "city-view",
                Category = "Comfort",
                IsActive = true,
                IsSystemDefined = true
            },

            // Entertainment
            new
            {
                Id = new RoomAmenityDefinitionId(Guid.Parse("20000000-0000-0000-0000-000000000006")),
                Code = "TV",
                Name = "Flat Screen TV",
                Icon = "tv",
                Category = "Entertainment",
                IsActive = true,
                IsSystemDefined = true
            },
            new
            {
                Id = new RoomAmenityDefinitionId(Guid.Parse("20000000-0000-0000-0000-000000000007")),
                Code = "STREAMING",
                Name = "Streaming Services",
                Icon = "streaming",
                Category = "Entertainment",
                IsActive = true,
                IsSystemDefined = true
            },
            new
            {
                Id = new RoomAmenityDefinitionId(Guid.Parse("20000000-0000-0000-0000-000000000008")),
                Code = "ROOM_WIFI",
                Name = "In-Room WiFi",
                Icon = "wifi",
                Category = "Entertainment",
                IsActive = true,
                IsSystemDefined = true
            },

            // Refreshments
            new
            {
                Id = new RoomAmenityDefinitionId(Guid.Parse("20000000-0000-0000-0000-000000000009")),
                Code = "MINIBAR",
                Name = "Mini Bar",
                Icon = "minibar",
                Category = "Refreshments",
                IsActive = true,
                IsSystemDefined = true
            },
            new
            {
                Id = new RoomAmenityDefinitionId(Guid.Parse("20000000-0000-0000-0000-00000000000A")),
                Code = "COFFEE_MAKER",
                Name = "Coffee Maker",
                Icon = "coffee",
                Category = "Refreshments",
                IsActive = true,
                IsSystemDefined = true
            },
            new
            {
                Id = new RoomAmenityDefinitionId(Guid.Parse("20000000-0000-0000-0000-00000000000B")),
                Code = "KETTLE",
                Name = "Electric Kettle",
                Icon = "kettle",
                Category = "Refreshments",
                IsActive = true,
                IsSystemDefined = true
            },

            // Bathroom
            new
            {
                Id = new RoomAmenityDefinitionId(Guid.Parse("20000000-0000-0000-0000-00000000000C")),
                Code = "BATHTUB",
                Name = "Bathtub",
                Icon = "bathtub",
                Category = "Bathroom",
                IsActive = true,
                IsSystemDefined = true
            },
            new
            {
                Id = new RoomAmenityDefinitionId(Guid.Parse("20000000-0000-0000-0000-00000000000D")),
                Code = "RAIN_SHOWER",
                Name = "Rain Shower",
                Icon = "shower",
                Category = "Bathroom",
                IsActive = true,
                IsSystemDefined = true
            },
            new
            {
                Id = new RoomAmenityDefinitionId(Guid.Parse("20000000-0000-0000-0000-00000000000E")),
                Code = "TOILETRIES",
                Name = "Premium Toiletries",
                Icon = "toiletries",
                Category = "Bathroom",
                IsActive = true,
                IsSystemDefined = true
            },
            new
            {
                Id = new RoomAmenityDefinitionId(Guid.Parse("20000000-0000-0000-0000-00000000000F")),
                Code = "HAIR_DRYER",
                Name = "Hair Dryer",
                Icon = "hairdryer",
                Category = "Bathroom",
                IsActive = true,
                IsSystemDefined = true
            },

            // Work
            new
            {
                Id = new RoomAmenityDefinitionId(Guid.Parse("20000000-0000-0000-0000-000000000010")),
                Code = "WORK_DESK",
                Name = "Work Desk",
                Icon = "desk",
                Category = "Work",
                IsActive = true,
                IsSystemDefined = true
            },

            // Safety
            new
            {
                Id = new RoomAmenityDefinitionId(Guid.Parse("20000000-0000-0000-0000-000000000011")),
                Code = "SAFE",
                Name = "In-Room Safe",
                Icon = "safe",
                Category = "Safety",
                IsActive = true,
                IsSystemDefined = true
            },

            // Sleep
            new
            {
                Id = new RoomAmenityDefinitionId(Guid.Parse("20000000-0000-0000-0000-000000000012")),
                Code = "KING_BED",
                Name = "King Size Bed",
                Icon = "bed",
                Category = "Sleep",
                IsActive = true,
                IsSystemDefined = true
            },
            new
            {
                Id = new RoomAmenityDefinitionId(Guid.Parse("20000000-0000-0000-0000-000000000013")),
                Code = "TWIN_BEDS",
                Name = "Twin Beds",
                Icon = "beds",
                Category = "Sleep",
                IsActive = true,
                IsSystemDefined = true
            },
            new
            {
                Id = new RoomAmenityDefinitionId(Guid.Parse("20000000-0000-0000-0000-000000000014")),
                Code = "BLACKOUT_CURTAINS",
                Name = "Blackout Curtains",
                Icon = "curtains",
                Category = "Sleep",
                IsActive = true,
                IsSystemDefined = true
            }
        };

        builder.Entity<RoomAmenityDefinition>().HasData(amenities);
    }

    private static void SeedRoomTypes(ModelBuilder builder)
    {
        var roomTypes = new List<object>
        {
            new
            {
                Id = new RoomTypeDefinitionId(Guid.Parse("30000000-0000-0000-0000-000000000001")),
                Code = "SINGLE",
                Name = "Single Room",
                Description = "Cozy room for one guest with a single bed",
                DefaultCapacity = 1,
                Icon = "single-room",
                IsActive = true,
                IsSystemDefined = true
            },
            new
            {
                Id = new RoomTypeDefinitionId(Guid.Parse("30000000-0000-0000-0000-000000000002")),
                Code = "DOUBLE",
                Name = "Double Room",
                Description = "Comfortable room with a double bed for two guests",
                DefaultCapacity = 2,
                Icon = "double-room",
                IsActive = true,
                IsSystemDefined = true
            },
            new
            {
                Id = new RoomTypeDefinitionId(Guid.Parse("30000000-0000-0000-0000-000000000003")),
                Code = "TWIN",
                Name = "Twin Room",
                Description = "Room with two single beds",
                DefaultCapacity = 2,
                Icon = "twin-room",
                IsActive = true,
                IsSystemDefined = true
            },
            new
            {
                Id = new RoomTypeDefinitionId(Guid.Parse("30000000-0000-0000-0000-000000000004")),
                Code = "TRIPLE",
                Name = "Triple Room",
                Description = "Spacious room accommodating three guests",
                DefaultCapacity = 3,
                Icon = "triple-room",
                IsActive = true,
                IsSystemDefined = true
            },
            new
            {
                Id = new RoomTypeDefinitionId(Guid.Parse("30000000-0000-0000-0000-000000000005")),
                Code = "QUAD",
                Name = "Quad Room",
                Description = "Large room for four guests",
                DefaultCapacity = 4,
                Icon = "quad-room",
                IsActive = true,
                IsSystemDefined = true
            },
            new
            {
                Id = new RoomTypeDefinitionId(Guid.Parse("30000000-0000-0000-0000-000000000006")),
                Code = "SUITE",
                Name = "Suite",
                Description = "Luxurious suite with separate living area",
                DefaultCapacity = 2,
                Icon = "suite",
                IsActive = true,
                IsSystemDefined = true
            },
            new
            {
                Id = new RoomTypeDefinitionId(Guid.Parse("30000000-0000-0000-0000-000000000007")),
                Code = "JUNIOR_SUITE",
                Name = "Junior Suite",
                Description = "Elegant suite with combined sleeping and sitting area",
                DefaultCapacity = 2,
                Icon = "junior-suite",
                IsActive = true,
                IsSystemDefined = true
            },
            new
            {
                Id = new RoomTypeDefinitionId(Guid.Parse("30000000-0000-0000-0000-000000000008")),
                Code = "PRESIDENTIAL_SUITE",
                Name = "Presidential Suite",
                Description = "Ultimate luxury with multiple rooms and premium amenities",
                DefaultCapacity = 4,
                Icon = "presidential-suite",
                IsActive = true,
                IsSystemDefined = true
            },
            new
            {
                Id = new RoomTypeDefinitionId(Guid.Parse("30000000-0000-0000-0000-000000000009")),
                Code = "FAMILY",
                Name = "Family Room",
                Description = "Spacious room designed for families with children",
                DefaultCapacity = 4,
                Icon = "family-room",
                IsActive = true,
                IsSystemDefined = true
            },
            new
            {
                Id = new RoomTypeDefinitionId(Guid.Parse("30000000-0000-0000-0000-00000000000A")),
                Code = "DELUXE",
                Name = "Deluxe Room",
                Description = "Premium room with enhanced amenities and extra space",
                DefaultCapacity = 2,
                Icon = "deluxe-room",
                IsActive = true,
                IsSystemDefined = true
            },
            new
            {
                Id = new RoomTypeDefinitionId(Guid.Parse("30000000-0000-0000-0000-00000000000B")),
                Code = "STUDIO",
                Name = "Studio",
                Description = "Self-contained room with kitchenette",
                DefaultCapacity = 2,
                Icon = "studio",
                IsActive = true,
                IsSystemDefined = true
            },
            new
            {
                Id = new RoomTypeDefinitionId(Guid.Parse("30000000-0000-0000-0000-00000000000C")),
                Code = "PENTHOUSE",
                Name = "Penthouse",
                Description = "Top-floor luxury accommodation with panoramic views",
                DefaultCapacity = 4,
                Icon = "penthouse",
                IsActive = true,
                IsSystemDefined = true
            }
        };

        builder.Entity<RoomTypeDefinition>().HasData(roomTypes);
    }
}