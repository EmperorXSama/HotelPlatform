using HotelManagement.BlazorServer.Services.Hotel;
using Microsoft.AspNetCore.Components;

namespace HotelManagement.BlazorServer.Components.Pages.Hotels;

public partial class HotelDetail : ComponentBase
{

    [Inject]
    public IHotelApiService HotelApiService { get; set; }
    
    
}