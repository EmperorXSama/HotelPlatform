// Application/Hotels/Queries/GetHotelById/GetHotelByIdQueryHandler.cs

using HotelPlatform.Application.Common.Interfaces.Repositories;
using HotelPlatform.Application.Common.Interfaces.Storage;
using HotelPlatform.Application.Features.Hotels.Common;
using HotelPlatform.Application.mapper;

namespace HotelPlatform.Application.Features.Hotels.Queries.GetById;

public class GetHotelByIdQueryHandler
    : IRequestHandler<GetHotelByIdQuery, ErrorOr<HotelDetailResponse>>
{
    private readonly IHotelRepository _hotelRepository;
    private readonly IFileUrlResolver _fileUrlResolver;
    private readonly IReferenceDataRepository _referenceDataRepository;
    public GetHotelByIdQueryHandler(
        IHotelRepository hotelRepository,
        IFileUrlResolver fileUrlResolver, IReferenceDataRepository referenceDataRepository)
    {
        _hotelRepository = hotelRepository;
        _fileUrlResolver = fileUrlResolver;
        _referenceDataRepository = referenceDataRepository;
    }

    public async Task<ErrorOr<HotelDetailResponse>> Handle(
        GetHotelByIdQuery request,
        CancellationToken cancellationToken)
    {
        var hotelId = (HotelId)request.HotelId;

        var hotel = await _hotelRepository.GetByIdWithDetailsAsync(
            hotelId, cancellationToken);

        if (hotel is null)
            return Error.NotFound(
                code: "Hotel.NotFound",
                description: $"Hotel with ID '{request.HotelId}' was not found.");
        var amenityIds = hotel.Amenities
            .Select(a => a.AmenityDefinitionId)
            .ToList();
        
        var amenityDefinitions = await _referenceDataRepository
            .GetHotelAmenitiesByIdsAsync(amenityIds, cancellationToken);

        return HotelsMapper.MapHotelToDetails(
            hotel, 
            _fileUrlResolver, 
            amenityDefinitions);
    }

   
}