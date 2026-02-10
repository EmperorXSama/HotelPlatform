using HotelManagement.BlazorServer.models.Filter;
using HotelManagement.BlazorServer.Models.Pagination;
using HotelManagement.BlazorServer.models.Response.Hotels;
using HotelManagement.BlazorServer.Services.Hotel;
using Microsoft.AspNetCore.Components;

namespace HotelManagement.BlazorServer.Components.Pages.Hotels;

public partial class HotelList : ComponentBase
{
    [Inject] private IHotelApiService HotelApiService { get; set; }
    [Inject] private NavigationManager NavigationManager { get; set; }

    private PagedResult<HotelSummaryResponse>? hotels = new PagedResult<HotelSummaryResponse>();
    private List<string> _errors = [];
    private bool _isLoading = false;

    private GetAllHotelSummaryFilter _queryParams = new()
    {
        PageSize = 9,
        Page = 1
    };

    protected override async Task OnInitializedAsync()
    {
        await LoadHotelsAsync();
    }

    private async Task LoadHotelsAsync(CancellationToken cancellationToken = default)
    {
        _isLoading = true;
        _errors = [];

        var result = await HotelApiService.GetAllAsync(_queryParams, cancellationToken);
        result.Switch(
            hotels => this.hotels = hotels,
            erros => _errors = erros.Select(e => e.Description).ToList()
            );
        
        _isLoading = false;
    }
}