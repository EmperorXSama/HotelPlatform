namespace HotelPlatform.Application.Features.Hotels.Common;

public sealed record RoomSummaryResponse(
    Guid Id,
    string Name,
    string? Description,
    int Capacity);