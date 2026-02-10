namespace HotelPlatform.Application.Features.Hotels.Common;

public sealed record RatingResponse(
    decimal AverageScore,
    int TotalCount);