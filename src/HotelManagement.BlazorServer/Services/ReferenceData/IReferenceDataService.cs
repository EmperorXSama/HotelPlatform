using HotelManagement.BlazorServer.models.Response.RefereneceData;

namespace HotelManagement.BlazorServer.Services.ReferenceData;

public interface IReferenceDataService
{
    Task<ErrorOr<List<AmenityResponse>>> GetHotelAmenitiesAsync(
        CancellationToken cancellationToken = default);
    Task<ErrorOr<List<AmenityResponse>>> GetRoomAmenitiesAsync(
        CancellationToken cancellationToken = default);

}