using HotelPlatform.Application.Common.Messaging;

namespace HotelPlatform.Application.Features.Hotels.Commands.CreateHotel;


public record CreateHotelCommand(
    string Name,
    string? Description,
    CreateHotelAddressDto? Address,
    List<CreateHotelPictureDto> Pictures,
    List<CreateHotelAmenityDto>? Amenities 
) : ICommand<ErrorOr<CreateHotelResult>>;
public sealed record CreateHotelAddressDto(
    string Street,
    string City,
    string Country,
    string? PostalCode,
    double? Latitude,
    double? Longitude);
public record CreateHotelAmenityDto(
    Guid AmenityDefinitionId,
    int UpchargeType,
    decimal UpchargeAmount,
    string? Currency
);
public sealed record CreateHotelPictureDto(
    Guid StoredFileId,
    string? AltText,
    bool IsMain);
public sealed record CreateHotelResult(
    Guid Id);