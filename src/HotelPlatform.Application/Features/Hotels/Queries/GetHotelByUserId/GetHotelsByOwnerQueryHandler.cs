using HotelPlatform.Application.Common.Interfaces;
using HotelPlatform.Application.Common.Interfaces.Repositories;
using HotelPlatform.Application.Common.Interfaces.Storage;
using HotelPlatform.Application.Features.Hotels.Common;
using HotelPlatform.Application.Features.Hotels.Queries.GetAllHotelSummary;
using HotelPlatform.Application.mapper;

namespace HotelPlatform.Application.Features.Hotels.Queries.GetHotelByUserId;

public class GetHotelsByOwnerQueryHandler : IRequestHandler<GetHotelsByOwnerQuery , ErrorOr<List<HotelDetailResponse>>>
{
    private readonly IHotelRepository _hotelRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IUserRepository _userRepository;
    private readonly IFileUrlResolver _fileUrlResolver;
    private readonly IReferenceDataRepository _referenceDataRepository;
    public GetHotelsByOwnerQueryHandler(IHotelRepository hotelRepository, ICurrentUser currentUser, IUserRepository userRepository, IFileUrlResolver fileUrlResolver, IReferenceDataRepository referenceDataRepository)
    {
        _hotelRepository = hotelRepository;
        _currentUser = currentUser;
        _userRepository = userRepository;
        _fileUrlResolver = fileUrlResolver;
        _referenceDataRepository = referenceDataRepository;
    }

    public async Task<ErrorOr<List<HotelDetailResponse>>> Handle(GetHotelsByOwnerQuery request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated || string.IsNullOrEmpty(_currentUser.Id))
        {
            return Error.Unauthorized(
                code: "User.Unauthorized",
                description: "User must be authenticated to create a hotel.");
        }
        
        var user = await _userRepository.GetByIdentityIdAsync(_currentUser.Id, cancellationToken);
        if (user is null)
        {
            return Error.NotFound(
                code: "User.NotFound",
                description: "User profile not found. Please complete registration first.");
        }

        var hotels = await _hotelRepository.GetByOwnerIdAsync(
            user.Id, cancellationToken);
        
        var allAmenityIds = hotels
            .SelectMany(h => h.Amenities)
            .Select(a => a.AmenityDefinitionId)
            .Distinct()
            .ToList();
        var amenityDefinitions = await _referenceDataRepository
            .GetHotelAmenitiesByIdsAsync(allAmenityIds, cancellationToken);
        
        return hotels
            .Select(h => HotelsMapper.MapHotelToDetails(
                h, 
                _fileUrlResolver, 
                amenityDefinitions))
            .ToList();
    }
}