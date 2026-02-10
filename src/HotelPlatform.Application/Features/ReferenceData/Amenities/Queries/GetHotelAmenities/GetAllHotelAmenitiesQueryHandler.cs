using HotelPlatform.Application.Common.Interfaces.Repositories;
using HotelPlatform.Application.Features.Common;

namespace HotelPlatform.Application.Features.ReferenceData.Amenities.Queries.GetHotelAmenities;

public class GetAllHotelAmenitiesQueryHandler : IRequestHandler<GetAllHotelAmenitiesQuery,ErrorOr<List<HotelAmenitiesResult>>>
{
    
    private readonly IReferenceDataRepository _referenceDataRepository;

    public GetAllHotelAmenitiesQueryHandler(IReferenceDataRepository referenceDataRepository)
    {
        _referenceDataRepository = referenceDataRepository;
    }

    public async Task<ErrorOr<List<HotelAmenitiesResult>>> Handle(GetAllHotelAmenitiesQuery request, CancellationToken cancellationToken)
    {
        var result = await _referenceDataRepository.GetActiveHotelAmenitiesAsync(cancellationToken);
        return result.Select(ha => new HotelAmenitiesResult(
            ha.Id,
            ha.Code,
            ha.Name,
            ha.Icon,
            ha.Category
        )).ToList();
    }
}