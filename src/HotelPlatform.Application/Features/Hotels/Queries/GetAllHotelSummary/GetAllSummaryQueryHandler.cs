using HotelPlatform.Application.Common.Interfaces.Repositories;
using HotelPlatform.Application.Common.Pagination;

namespace HotelPlatform.Application.Features.Hotels.Queries.GetAllHotelSummary;

public class GetAllSummaryQueryHandler : IRequestHandler<GetAllHotelSummaryQuery,ErrorOr<PagedResult<HotelSummaryResponse>>>
{
    private IHotelRepository _hotelRepository;

    public GetAllSummaryQueryHandler(IHotelRepository hotelRepository)
    {
        _hotelRepository = hotelRepository;
    }

    public async Task<ErrorOr<PagedResult<HotelSummaryResponse>>> Handle(GetAllHotelSummaryQuery request, CancellationToken cancellationToken)
    {
        var result = await _hotelRepository.GetPagedSummaryHotelAsync(request.Filter, cancellationToken);
        if (result.IsError) return result.Errors;
        
        return result;
    }
}