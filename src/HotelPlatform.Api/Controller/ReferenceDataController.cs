using HotelPlatform.Application.Features.ReferenceData.Amenities.Commands;
using HotelPlatform.Application.Features.ReferenceData.Amenities.Queries.GetHotelAmenities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelPlatform.Api.Controller;

[Route("api/reference-data")]
public class ReferenceDataController : ApiBaseController
{
    private readonly ISender _sender;

    public ReferenceDataController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("hotel-amenities")]
    public async Task<IActionResult> GetAllHotelAmenities(CancellationToken cancellationToken = default)
    {
        var amenities = await _sender.Send(new GetAllHotelAmenitiesQuery());
        return  ToApiResponse(amenities);
    }

    [HttpPost("{amenityId}/UpdateUpcharge")]
    [Authorize(Policy = "HotelOwner")]
    public async Task<IActionResult> UpdateUpcharge(Guid amenityId,[FromBody] UpdateAmenityUpchargeRequest upcharge)
    {
       var command = new ChangeHotelSelectedAmenityUpcharge(upcharge.HotelId,amenityId,upcharge.UpchargeType,upcharge.Amount,upcharge.CurrencyCode);
       var result = await _sender.Send(command);
       return ToApiResponse(result);
    }

 
}

public class UpdateAmenityUpchargeRequest
{
    public Guid HotelId { get; set; }
    public int UpchargeType { get; set; }
    public decimal Amount { get; set; }
    public string? CurrencyCode { get; set; }
}