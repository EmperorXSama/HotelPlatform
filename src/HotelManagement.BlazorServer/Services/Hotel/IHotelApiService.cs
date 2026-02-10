// Services/Hotels/IHotelApiService.cs

using HotelManagement.BlazorServer.models.Filter;
using HotelManagement.BlazorServer.Models.Pagination;
using HotelManagement.BlazorServer.models.Request.Hotels;
using HotelManagement.BlazorServer.models.Response.Hotels;

namespace HotelManagement.BlazorServer.Services.Hotel;

public interface IHotelApiService
{
    Task<ErrorOr<HotelResponse>> GetByIdAsync(
        Guid id, 
        CancellationToken cancellationToken = default);

    Task<ErrorOr<List<HotelDetailResponse>>> GetByOwnerIdAsync(CancellationToken cancellationToken = default);
    
    Task<ErrorOr<PagedResult<HotelSummaryResponse>>> GetAllAsync(
        GetAllHotelSummaryFilter request,
        CancellationToken cancellationToken = default);
    
    Task<ErrorOr<CreateHotelResponse>> CreateAsync(
        CreateHotelRequest request, 
        CancellationToken cancellationToken = default);
    
    Task<ErrorOr<HotelResponse>> UpdateAsync(
        Guid id, 
        UpdateHotelRequest request, 
        CancellationToken cancellationToken = default);
    
    Task<ErrorOr<Success>> DeleteAsync(
        Guid id, 
        CancellationToken cancellationToken = default);
    
    Task<ErrorOr<List<Guid>>> UpdateHotelAmenities(List<Guid> selectedAmenities, Guid hotelId,
        CancellationToken cancellationToken = default);
    Task<ErrorOr<List<Guid>>> UpdateRoomAmenities(List<Guid> selectedAmenities, Guid roomId,
        CancellationToken cancellationToken = default);
}