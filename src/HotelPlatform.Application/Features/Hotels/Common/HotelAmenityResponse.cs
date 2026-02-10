namespace HotelPlatform.Application.Features.Hotels.Common;


    
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
