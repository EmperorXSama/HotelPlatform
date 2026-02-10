namespace HotelPlatform.Api.Dto.RequestObjects;


public sealed record CreateHotelRequest(
    string Name,
    string? Description,
    CreateHotelAddressRequest? Address,
    List<CreateHotelPictureRequest> Pictures,
    List<CreateHotelAmenityRequest>? Amenities  // ADD THIS
);
public sealed record CreateHotelAmenityRequest(
    Guid AmenityDefinitionId,
    int UpchargeType,
    decimal UpchargeAmount,
    string? Currency
);

public sealed record CreateHotelAddressRequest(
    string Street,
    string City,
    string Country,
    string? PostalCode,
    double? Latitude,
    double? Longitude);

public sealed record CreateHotelPictureRequest(
    Guid StoredFileId,
    string? AltText,
    bool IsMain);