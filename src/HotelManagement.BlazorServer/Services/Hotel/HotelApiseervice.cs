// Services/Hotels/HotelApiService.cs

using HotelManagement.BlazorServer.Http;
using HotelManagement.BlazorServer.models.Filter;
using HotelManagement.BlazorServer.Models.Pagination;
using HotelManagement.BlazorServer.models.Request.Hotels;
using HotelManagement.BlazorServer.models.Response.Hotels;

namespace HotelManagement.BlazorServer.Services.Hotel;

public sealed class HotelApiService : IHotelApiService
{
    private readonly IApiClient _apiClient;
    private const string BaseEndpoint = "api/hotels";

    public HotelApiService(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<ErrorOr<HotelResponse>> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _apiClient.GetAsync<HotelResponse>(
            $"{BaseEndpoint}/{id}",
            cancellationToken);
    }

    public async Task<ErrorOr<List<HotelDetailResponse>>> GetByOwnerIdAsync(CancellationToken cancellationToken = default)
    {
        return await _apiClient.GetAsync<List<HotelDetailResponse>>($"{BaseEndpoint}/my-hotels" ,cancellationToken);
    }

    public async Task<ErrorOr<PagedResult<HotelSummaryResponse>>> GetAllAsync(
        GetAllHotelSummaryFilter request,
        CancellationToken cancellationToken = default)
    {
        return await _apiClient.GetPagedAsync<HotelSummaryResponse>(
            BaseEndpoint,
            request,
            cancellationToken);
    }

    public async Task<ErrorOr<CreateHotelResponse>> CreateAsync(
        CreateHotelRequest request,
        CancellationToken cancellationToken = default)
    {
        return await _apiClient.PostAsync<CreateHotelRequest, CreateHotelResponse>(
            BaseEndpoint,
            request,
            cancellationToken);
    }
    public async Task<ErrorOr<HotelResponse>> UpdateAsync(
        Guid id,
        UpdateHotelRequest request,
        CancellationToken cancellationToken = default)
    {
        return await _apiClient.PutAsync<UpdateHotelRequest, HotelResponse>(
            $"{BaseEndpoint}/{id}",
            request,
            cancellationToken);
    }

    public async Task<ErrorOr<Success>> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _apiClient.DeleteAsync(
            $"{BaseEndpoint}/{id}",
            cancellationToken);
    }


    public async Task<ErrorOr<List<Guid>>> UpdateHotelAmenities(List<Guid>selectedAmenities, Guid hotelId, CancellationToken cancellationToken = default)
    {
        var response =  await _apiClient.PutAsync<List<Guid>,List<Guid>>(
            $"{BaseEndpoint}/{hotelId}", 
            selectedAmenities,
            cancellationToken);
        
        return response;
    }

    public async Task<ErrorOr<List<Guid>>>  UpdateRoomAmenities(List<Guid> selectedAmenities, Guid roomId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
