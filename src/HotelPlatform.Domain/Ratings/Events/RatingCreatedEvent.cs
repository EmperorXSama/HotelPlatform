using HotelPlatform.Domain.Common.StronglyTypedIds;

namespace HotelPlatform.Domain.Ratings.Events;

public sealed record RatingCreatedEvent(
    RatingId RatingId,
    HotelId HotelId,
    UserId UserId,
    int Score) : BaseDomainEvent;