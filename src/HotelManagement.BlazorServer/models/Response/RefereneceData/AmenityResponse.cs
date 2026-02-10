namespace HotelManagement.BlazorServer.models.Response.RefereneceData;

public class AmenityResponse
{
    public Guid hotelAmenityId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? IconCode { get; set; }
    public string? Category { get; set; }
}