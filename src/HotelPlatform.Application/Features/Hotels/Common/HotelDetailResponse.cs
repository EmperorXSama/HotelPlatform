namespace HotelPlatform.Application.Features.Hotels.Common;

public sealed record HotelDetailResponse(
    Guid Id,
    string Name,
    string? Description,
    string Status,
    AddressResponse? Address,
    RatingResponse Rating,
    List<HotelPictureResponse> Pictures,
    List<HotelAmenityResponse> Amenities,
    List<RoomSummaryResponse> Rooms);