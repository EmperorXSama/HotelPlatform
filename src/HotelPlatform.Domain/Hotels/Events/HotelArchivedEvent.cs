using HotelPlatform.Domain.Common.StronglyTypedIds;

namespace HotelPlatform.Domain.Hotels.Events;

public sealed record HotelArchivedEvent(
    HotelId HotelId,
    UserId OwnerId) : BaseDomainEvent;