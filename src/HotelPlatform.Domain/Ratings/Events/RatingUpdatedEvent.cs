using HotelPlatform.Domain.Common.StronglyTypedIds;

namespace HotelPlatform.Domain.Ratings.Events;

public sealed record RatingUpdatedEvent(
    RatingId RatingId,
    HotelId HotelId,
    UserId UserId,
    int OldScore,
    int NewScore) : BaseDomainEvent;