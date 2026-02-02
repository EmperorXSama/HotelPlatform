using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HotelPlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "hotel_amenity_definitions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    icon = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    is_system_defined = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_hotel_amenity_definitions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Hotels",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    owner_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    address_street = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    address_city = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    address_country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    address_postal_code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    address_latitude = table.Column<double>(type: "double precision", nullable: true),
                    address_longitude = table.Column<double>(type: "double precision", nullable: true),
                    rating_average = table.Column<decimal>(type: "numeric(3,2)", precision: 3, scale: 2, nullable: false, defaultValue: 0m),
                    rating_total_counts = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    last_modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hotels", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ratings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    HotelId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Score = table.Column<int>(type: "integer", nullable: false),
                    Comment = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ratings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "room_amenity_definitions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    icon = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    is_system_defined = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_room_amenity_definitions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "room_type_definitions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    default_capacity = table.Column<int>(type: "integer", nullable: false),
                    icon = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    is_system_defined = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_room_type_definitions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "stored_files",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    owner_id = table.Column<Guid>(type: "uuid", nullable: false),
                    original_file_name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    stored_file_name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    content_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    size_in_bytes = table.Column<long>(type: "bigint", nullable: false),
                    url = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    StorageProvider = table.Column<string>(type: "text", nullable: false),
                    BlobPath = table.Column<string>(type: "text", nullable: true),
                    created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stored_files", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    identity_id = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    display_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    last_modified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "hotel_pictures",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    stored_file_id = table.Column<Guid>(type: "uuid", nullable: false),
                    alt_text = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    is_main = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<int>(type: "integer", nullable: false),
                    hotel_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_hotel_pictures", x => x.id);
                    table.ForeignKey(
                        name: "FK_hotel_pictures_Hotels_hotel_id",
                        column: x => x.hotel_id,
                        principalTable: "Hotels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "hotel_selected_amenities",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    amenity_definition_id = table.Column<Guid>(type: "uuid", nullable: false),
                    upcharge_type = table.Column<int>(type: "integer", nullable: false),
                    upcharge_amount = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    upcharge_currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: true),
                    hotel_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_hotel_selected_amenities", x => x.id);
                    table.ForeignKey(
                        name: "FK_hotel_selected_amenities_Hotels_hotel_id",
                        column: x => x.hotel_id,
                        principalTable: "Hotels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Rooms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    room_type_id = table.Column<Guid>(type: "uuid", nullable: false),
                    capacity = table.Column<int>(type: "integer", nullable: false),
                    base_price_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    base_price_currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    hotel_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.id);
                    table.ForeignKey(
                        name: "FK_Rooms_Hotels_hotel_id",
                        column: x => x.hotel_id,
                        principalTable: "Hotels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "room_pictures",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    stored_file_id = table.Column<Guid>(type: "uuid", nullable: false),
                    alt_text = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    is_main = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<int>(type: "integer", nullable: false),
                    room_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_room_pictures", x => x.id);
                    table.ForeignKey(
                        name: "FK_room_pictures_Rooms_room_id",
                        column: x => x.room_id,
                        principalTable: "Rooms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "room_selected_amenities",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    amenity_definition_id = table.Column<Guid>(type: "uuid", nullable: false),
                    upcharge_type = table.Column<int>(type: "integer", nullable: false),
                    upcharge_amount = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    upcharge_currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: true),
                    room_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_room_selected_amenities", x => x.id);
                    table.ForeignKey(
                        name: "FK_room_selected_amenities_Rooms_room_id",
                        column: x => x.room_id,
                        principalTable: "Rooms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "hotel_amenity_definitions",
                columns: new[] { "id", "category", "code", "icon", "is_active", "is_system_defined", "name" },
                values: new object[,]
                {
                    { new Guid("10000000-0000-0000-0000-000000000001"), "Recreation", "POOL", "pool", true, true, "Swimming Pool" },
                    { new Guid("10000000-0000-0000-0000-000000000002"), "Recreation", "GYM", "gym", true, true, "Fitness Center" },
                    { new Guid("10000000-0000-0000-0000-000000000003"), "Recreation", "SPA", "spa", true, true, "Spa & Wellness" },
                    { new Guid("10000000-0000-0000-0000-000000000004"), "Services", "RESTAURANT", "restaurant", true, true, "Restaurant" },
                    { new Guid("10000000-0000-0000-0000-000000000005"), "Services", "BAR", "bar", true, true, "Bar & Lounge" },
                    { new Guid("10000000-0000-0000-0000-000000000006"), "Services", "ROOM_SERVICE", "room-service", true, true, "Room Service" },
                    { new Guid("10000000-0000-0000-0000-000000000007"), "Services", "CONCIERGE", "concierge", true, true, "Concierge Service" },
                    { new Guid("10000000-0000-0000-0000-000000000008"), "Services", "LAUNDRY", "laundry", true, true, "Laundry Service" },
                    { new Guid("10000000-0000-0000-0000-000000000009"), "Facilities", "PARKING", "parking", true, true, "Parking" },
                    { new Guid("10000000-0000-0000-0000-00000000000a"), "Facilities", "WIFI", "wifi", true, true, "Free WiFi" },
                    { new Guid("10000000-0000-0000-0000-00000000000b"), "Facilities", "BUSINESS_CENTER", "business", true, true, "Business Center" },
                    { new Guid("10000000-0000-0000-0000-00000000000c"), "Facilities", "MEETING_ROOMS", "meeting", true, true, "Meeting Rooms" },
                    { new Guid("10000000-0000-0000-0000-00000000000d"), "Accessibility", "WHEELCHAIR", "wheelchair", true, true, "Wheelchair Accessible" },
                    { new Guid("10000000-0000-0000-0000-00000000000e"), "Accessibility", "ELEVATOR", "elevator", true, true, "Elevator" },
                    { new Guid("10000000-0000-0000-0000-00000000000f"), "Family", "KIDS_CLUB", "kids", true, true, "Kids Club" },
                    { new Guid("10000000-0000-0000-0000-000000000010"), "Family", "BABYSITTING", "babysitting", true, true, "Babysitting Service" },
                    { new Guid("10000000-0000-0000-0000-000000000011"), "Pets", "PET_FRIENDLY", "pet", true, true, "Pet Friendly" }
                });

            migrationBuilder.InsertData(
                table: "room_amenity_definitions",
                columns: new[] { "id", "category", "code", "icon", "is_active", "is_system_defined", "name" },
                values: new object[,]
                {
                    { new Guid("20000000-0000-0000-0000-000000000001"), "Comfort", "AC", "ac", true, true, "Air Conditioning" },
                    { new Guid("20000000-0000-0000-0000-000000000002"), "Comfort", "HEATING", "heating", true, true, "Heating" },
                    { new Guid("20000000-0000-0000-0000-000000000003"), "Comfort", "BALCONY", "balcony", true, true, "Balcony" },
                    { new Guid("20000000-0000-0000-0000-000000000004"), "Comfort", "SEA_VIEW", "sea-view", true, true, "Sea View" },
                    { new Guid("20000000-0000-0000-0000-000000000005"), "Comfort", "CITY_VIEW", "city-view", true, true, "City View" },
                    { new Guid("20000000-0000-0000-0000-000000000006"), "Entertainment", "TV", "tv", true, true, "Flat Screen TV" },
                    { new Guid("20000000-0000-0000-0000-000000000007"), "Entertainment", "STREAMING", "streaming", true, true, "Streaming Services" },
                    { new Guid("20000000-0000-0000-0000-000000000008"), "Entertainment", "ROOM_WIFI", "wifi", true, true, "In-Room WiFi" },
                    { new Guid("20000000-0000-0000-0000-000000000009"), "Refreshments", "MINIBAR", "minibar", true, true, "Mini Bar" },
                    { new Guid("20000000-0000-0000-0000-00000000000a"), "Refreshments", "COFFEE_MAKER", "coffee", true, true, "Coffee Maker" },
                    { new Guid("20000000-0000-0000-0000-00000000000b"), "Refreshments", "KETTLE", "kettle", true, true, "Electric Kettle" },
                    { new Guid("20000000-0000-0000-0000-00000000000c"), "Bathroom", "BATHTUB", "bathtub", true, true, "Bathtub" },
                    { new Guid("20000000-0000-0000-0000-00000000000d"), "Bathroom", "RAIN_SHOWER", "shower", true, true, "Rain Shower" },
                    { new Guid("20000000-0000-0000-0000-00000000000e"), "Bathroom", "TOILETRIES", "toiletries", true, true, "Premium Toiletries" },
                    { new Guid("20000000-0000-0000-0000-00000000000f"), "Bathroom", "HAIR_DRYER", "hairdryer", true, true, "Hair Dryer" },
                    { new Guid("20000000-0000-0000-0000-000000000010"), "Work", "WORK_DESK", "desk", true, true, "Work Desk" },
                    { new Guid("20000000-0000-0000-0000-000000000011"), "Safety", "SAFE", "safe", true, true, "In-Room Safe" },
                    { new Guid("20000000-0000-0000-0000-000000000012"), "Sleep", "KING_BED", "bed", true, true, "King Size Bed" },
                    { new Guid("20000000-0000-0000-0000-000000000013"), "Sleep", "TWIN_BEDS", "beds", true, true, "Twin Beds" },
                    { new Guid("20000000-0000-0000-0000-000000000014"), "Sleep", "BLACKOUT_CURTAINS", "curtains", true, true, "Blackout Curtains" }
                });

            migrationBuilder.InsertData(
                table: "room_type_definitions",
                columns: new[] { "id", "code", "default_capacity", "description", "icon", "is_active", "is_system_defined", "name" },
                values: new object[,]
                {
                    { new Guid("30000000-0000-0000-0000-000000000001"), "SINGLE", 1, "Cozy room for one guest with a single bed", "single-room", true, true, "Single Room" },
                    { new Guid("30000000-0000-0000-0000-000000000002"), "DOUBLE", 2, "Comfortable room with a double bed for two guests", "double-room", true, true, "Double Room" },
                    { new Guid("30000000-0000-0000-0000-000000000003"), "TWIN", 2, "Room with two single beds", "twin-room", true, true, "Twin Room" },
                    { new Guid("30000000-0000-0000-0000-000000000004"), "TRIPLE", 3, "Spacious room accommodating three guests", "triple-room", true, true, "Triple Room" },
                    { new Guid("30000000-0000-0000-0000-000000000005"), "QUAD", 4, "Large room for four guests", "quad-room", true, true, "Quad Room" },
                    { new Guid("30000000-0000-0000-0000-000000000006"), "SUITE", 2, "Luxurious suite with separate living area", "suite", true, true, "Suite" },
                    { new Guid("30000000-0000-0000-0000-000000000007"), "JUNIOR_SUITE", 2, "Elegant suite with combined sleeping and sitting area", "junior-suite", true, true, "Junior Suite" },
                    { new Guid("30000000-0000-0000-0000-000000000008"), "PRESIDENTIAL_SUITE", 4, "Ultimate luxury with multiple rooms and premium amenities", "presidential-suite", true, true, "Presidential Suite" },
                    { new Guid("30000000-0000-0000-0000-000000000009"), "FAMILY", 4, "Spacious room designed for families with children", "family-room", true, true, "Family Room" },
                    { new Guid("30000000-0000-0000-0000-00000000000a"), "DELUXE", 2, "Premium room with enhanced amenities and extra space", "deluxe-room", true, true, "Deluxe Room" },
                    { new Guid("30000000-0000-0000-0000-00000000000b"), "STUDIO", 2, "Self-contained room with kitchenette", "studio", true, true, "Studio" },
                    { new Guid("30000000-0000-0000-0000-00000000000c"), "PENTHOUSE", 4, "Top-floor luxury accommodation with panoramic views", "penthouse", true, true, "Penthouse" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_hotel_amenity_definitions_code",
                table: "hotel_amenity_definitions",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_hotel_amenity_definitions_is_active",
                table: "hotel_amenity_definitions",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "IX_hotel_pictures_hotel_id_stored_file_id",
                table: "hotel_pictures",
                columns: new[] { "hotel_id", "stored_file_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_hotel_selected_amenities_hotel_id_amenity_definition_id",
                table: "hotel_selected_amenities",
                columns: new[] { "hotel_id", "amenity_definition_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Hotels_name",
                table: "Hotels",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "IX_Hotels_owner_id",
                table: "Hotels",
                column: "owner_id");

            migrationBuilder.CreateIndex(
                name: "IX_Hotels_status",
                table: "Hotels",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_ratings_HotelId",
                table: "ratings",
                column: "HotelId");

            migrationBuilder.CreateIndex(
                name: "IX_ratings_HotelId_UserId",
                table: "ratings",
                columns: new[] { "HotelId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ratings_UserId",
                table: "ratings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_room_amenity_definitions_code",
                table: "room_amenity_definitions",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_room_amenity_definitions_is_active",
                table: "room_amenity_definitions",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "IX_room_pictures_room_id_stored_file_id",
                table: "room_pictures",
                columns: new[] { "room_id", "stored_file_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_room_selected_amenities_room_id_amenity_definition_id",
                table: "room_selected_amenities",
                columns: new[] { "room_id", "amenity_definition_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_room_type_definitions_code",
                table: "room_type_definitions",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_room_type_definitions_is_active",
                table: "room_type_definitions",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_hotel_id",
                table: "Rooms",
                column: "hotel_id");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_room_type_id",
                table: "Rooms",
                column: "room_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_stored_files_owner_id",
                table: "stored_files",
                column: "owner_id");

            migrationBuilder.CreateIndex(
                name: "IX_stored_files_stored_file_name",
                table: "stored_files",
                column: "stored_file_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_email",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_identity_id",
                table: "users",
                column: "identity_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "hotel_amenity_definitions");

            migrationBuilder.DropTable(
                name: "hotel_pictures");

            migrationBuilder.DropTable(
                name: "hotel_selected_amenities");

            migrationBuilder.DropTable(
                name: "ratings");

            migrationBuilder.DropTable(
                name: "room_amenity_definitions");

            migrationBuilder.DropTable(
                name: "room_pictures");

            migrationBuilder.DropTable(
                name: "room_selected_amenities");

            migrationBuilder.DropTable(
                name: "room_type_definitions");

            migrationBuilder.DropTable(
                name: "stored_files");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "Rooms");

            migrationBuilder.DropTable(
                name: "Hotels");
        }
    }
}
