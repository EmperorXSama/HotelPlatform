// HotelsMapper.cs

using HotelPlatform.Application.Common.Interfaces.Storage;
using HotelPlatform.Application.Features.Hotels.Common;
using HotelPlatform.Domain.ReferenceData;

namespace HotelPlatform.Application.mapper;

public static class HotelsMapper
{
    public static HotelDetailResponse MapHotelToDetails(
        Hotel hotel, 
        IFileUrlResolver fileUrlResolver,
        IReadOnlyList<HotelAmenityDefinition> amenityDefinitions) // ← Add this
    {
        // Build amenity responses by joining selected amenities with definitions
        var amenityResponses = hotel.Amenities
            .Select(selectedAmenity =>
            {
                var definition = amenityDefinitions
                    .FirstOrDefault(d => d.Id == selectedAmenity.AmenityDefinitionId);

                if (definition is null)
                {
                    // Handle missing definition - you could log this or skip
                    return null;
                }

                return new HotelAmenityResponse(
                    AmenityDefinitionId: selectedAmenity.AmenityDefinitionId.Value,
                    Code: definition.Code,
                    Name: definition.Name,
                    IconCode: definition.Icon,
                    Category: definition.Category,
                    UpchargeType: (int)selectedAmenity.Upcharge.Type,
                    UpchargeAmount: selectedAmenity.Upcharge.Amount,
                    Currency: selectedAmenity.Upcharge.Currency?.Code
                );
            })
            .Where(a => a is not null) // Filter out nulls
            .Cast<HotelAmenityResponse>()
            .ToList();

        return new HotelDetailResponse(
            Id: hotel.Id.Value,
            Name: hotel.Name,
            Description: hotel.Description,
            Status: hotel.Status.ToString(),
            Address: hotel.Address is not null
                ? new AddressResponse(
                    hotel.Address.Street,
                    hotel.Address.City,
                    hotel.Address.Country,
                    hotel.Address.PostalCode)
                : null,
            Rating: new RatingResponse(
                hotel.AggregatedRating.AverageScore,
                hotel.AggregatedRating.TotalCounts),
            Pictures: hotel.Pictures
                .Select((p, index) => new HotelPictureResponse(
                    StoredFileId: p.StoredFileId.Value,
                    Url: fileUrlResolver.GetAccessUrl(p.StoredFileId),
                    IsMain: p.IsMain,
                    SortOrder: index))
                .OrderByDescending(p => p.IsMain)
                .ToList(),
            Amenities: amenityResponses,
            Rooms: hotel.Rooms
                .Select(r => new RoomSummaryResponse(
                    r.Id.Value,
                    r.Name,
                    r.Description,
                    r.Capacity))
                .ToList());
    }
}