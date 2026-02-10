using HotelPlatform.Application.Features.Hotels.Common;
using HotelPlatform.Application.Features.Hotels.Queries.GetAllHotelSummary;

namespace HotelPlatform.Application.Features.Hotels.Queries.GetHotelByUserId;

public record GetHotelsByOwnerQuery : IQuery<ErrorOr<List<HotelDetailResponse>>>;