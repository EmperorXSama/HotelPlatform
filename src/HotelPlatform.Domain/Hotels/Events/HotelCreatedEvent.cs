using HotelPlatform.Domain.Common.StronglyTypedIds;

namespace HotelPlatform.Domain.Hotels.Events;

public sealed record HotelCreatedEvent(
    HotelId HotelId,
    UserId OwnerId) : BaseDomainEvent;