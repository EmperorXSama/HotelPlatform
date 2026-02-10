namespace HotelManagement.BlazorServer.models.Response.Hotels;

/// <summary>
/// Response model for hotel details
/// </summary>
public sealed record HotelResponse(
    Guid Id,
    string Name,
    string? Description,
    string Status,
    HotelAddressResponse? Address,
    List<HotelPictureResponse> Pictures,
    List<HotelAmenityResponse> Amenities,
    HotelRatingResponse? Rating,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

/// <summary>
/// Address details in hotel response
/// </summary>
public sealed record HotelAddressResponse(
    string Street,
    string City,
    string Country,
    string? PostalCode,
    double? Latitude,
    double? Longitude
);

/// <summary>
/// Amenity details in hotel response (includes definition info + upcharge)
/// </summary>
public sealed record HotelAmenityResponse(
    Guid AmenityDefinitionId,
    string Code,
    string Name,
    string? IconCode,
    string? Category,
    int UpchargeType,
    decimal UpchargeAmount,
    string? Currency
);

/// <summary>
/// Rating summary in hotel response
/// </summary>
public sealed record HotelRatingResponse(
    decimal AverageRating,
    int TotalReviews
);

public sealed record CreateHotelResponse(Guid Id);