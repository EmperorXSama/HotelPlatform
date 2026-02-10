namespace HotelPlatform.Application.Features.Common;

public record HotelAmenitiesResult(
    Guid HotelAmenityId,
    string Code,
    string Name,
    string? IconCode,
    string? Category
);
    
