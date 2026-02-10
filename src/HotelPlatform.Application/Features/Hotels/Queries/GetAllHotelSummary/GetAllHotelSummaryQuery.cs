using HotelPlatform.Application.Common.Pagination;

namespace HotelPlatform.Application.Features.Hotels.Queries.GetAllHotelSummary;

public record GetAllHotelSummaryQuery(
    GetAllHotelSummaryFilter Filter
    )
    :IPublicQuery<ErrorOr<PagedResult<HotelSummaryResponse>>>;