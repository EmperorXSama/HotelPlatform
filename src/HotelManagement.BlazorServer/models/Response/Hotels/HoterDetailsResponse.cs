namespace HotelManagement.BlazorServer.models.Response.Hotels;

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
    
public sealed record AddressResponse(
    string Street,
    string City,
    string Country,
    string? PostalCode);
public sealed record RatingResponse(
    decimal AverageScore,
    int TotalCount);
    
public sealed record RoomSummaryResponse(
    Guid Id,
    string Name,
    string? Description,
    int Capacity);