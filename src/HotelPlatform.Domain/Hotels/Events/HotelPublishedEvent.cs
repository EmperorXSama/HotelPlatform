using HotelPlatform.Domain.Common.StronglyTypedIds;

namespace HotelPlatform.Domain.Hotels.Events;

public sealed record HotelPublishedEvent(
    HotelId HotelId,
    UserId OwnerId) : BaseDomainEvent;