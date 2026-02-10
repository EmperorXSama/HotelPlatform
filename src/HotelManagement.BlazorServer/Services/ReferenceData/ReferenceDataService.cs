using HotelManagement.BlazorServer.Http;
using HotelManagement.BlazorServer.models.Response.RefereneceData;

namespace HotelManagement.BlazorServer.Services.ReferenceData;

public class ReferenceDataService : IReferenceDataService
{
    private readonly IApiClient _apiClient;
    private const string BaseEndpoint = "api/reference-data";
    private const string AmenitiesEndpoint = "hotel-amenities";
    public ReferenceDataService(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<ErrorOr<List<AmenityResponse>>> GetHotelAmenitiesAsync(
        CancellationToken cancellationToken = default)
    {
        return await _apiClient.GetAsync<List<AmenityResponse>>(
            $"{BaseEndpoint}/hotel-amenities", 
            cancellationToken);
    }

    public async  Task<ErrorOr<List<AmenityResponse>>> GetRoomAmenitiesAsync(CancellationToken cancellationToken = default)
    {
        // todo : implement room 
        return await _apiClient.GetAsync<List<AmenityResponse>>(
            $"{BaseEndpoint}/hotel-amenities", 
            cancellationToken);
    }

}