using HotelPlatform.Application.Features.Hotels.Common;

namespace HotelPlatform.Application.Features.Hotels.Queries.GetById;

public sealed record GetHotelByIdQuery(Guid HotelId) 
    : IPublicQuery<ErrorOr<HotelDetailResponse>>;