using HotelPlatform.Application.Common.Messaging;

namespace HotelPlatform.Application.Features.Hotels.Commands.CreateHotel;


public sealed record CreateHotelCommand(
    string Name,
    string? Description,
    CreateHotelAddressDto? Address,
    List<CreateHotelPictureDto> Pictures) : IRequest<ErrorOr<CreateHotelResult>>;
public sealed record CreateHotelAddressDto(
    string Street,
    string City,
    string Country,
    string? PostalCode,
    double? Latitude,
    double? Longitude);

public sealed record CreateHotelPictureDto(
    Guid StoredFileId,
    string? AltText,
    bool IsMain);
public sealed record CreateHotelResult(
    Guid Id,
    string Name,
    string? Description,
    string Status,
    string? MainPictureUrl,
    int PictureCount,
    DateTimeOffset CreatedAt);