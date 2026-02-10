using HotelPlatform.Application.Features.Common;
using HotelPlatform.Application.Features.Hotels.Common;

namespace HotelPlatform.Application.Features.ReferenceData.Amenities.Queries.GetHotelAmenities;

public record GetAllHotelAmenitiesQuery():
    IPublicQuery<ErrorOr<List<HotelAmenitiesResult>>>;