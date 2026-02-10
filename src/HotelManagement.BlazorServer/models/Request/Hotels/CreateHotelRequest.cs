namespace HotelManagement.BlazorServer.models.Request.Hotels;

/// <summary>
/// Request to create a new hotel
/// </summary>
public sealed record CreateHotelRequest(
    string Name,
    string? Description,
    CreateHotelAddressRequest? Address,
    List<CreateHotelPictureRequest> Pictures,
    List<CreateHotelAmenityRequest>? Amenities = null
);

/// <summary>
/// Address information for hotel creation
/// </summary>
public sealed record CreateHotelAddressRequest(
    string Street,
    string City,
    string Country,
    string? PostalCode,
    double? Latitude,
    double? Longitude
);

/// <summary>
/// Picture information for hotel creation
/// </summary>
public sealed record CreateHotelPictureRequest(
    Guid StoredFileId,
    string? AltText,
    bool IsMain
);

/// <summary>
/// Amenity information for hotel creation
/// </summary>
public sealed record CreateHotelAmenityRequest(
    Guid AmenityDefinitionId,
    int UpchargeType,
    decimal UpchargeAmount,
    string? Currency
);

/// <summary>
/// Request to update an existing hotel
/// </summary>
public sealed record UpdateHotelRequest(
    string Name,
    string? Description,
    CreateHotelAddressRequest? Address
);