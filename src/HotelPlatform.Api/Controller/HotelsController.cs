// Api/Controllers/HotelsController.cs

using ErrorOr;
using HotelPlatform.Api.Dto.RequestObjects;
using HotelPlatform.Application.Features.Hotels.Commands.CreateHotel;
using HotelPlatform.Application.Features.Hotels.Commands.UpdateAmenities;
using HotelPlatform.Application.Features.Hotels.Queries.GetAllHotelSummary;
using HotelPlatform.Application.Features.Hotels.Queries.GetById;
using HotelPlatform.Application.Features.Hotels.Queries.GetHotelByUserId;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelPlatform.Api.Controller;

[Tags("Hotels")]
public class HotelsController : ApiBaseController
{
    private readonly ISender _sender;
    private ILogger<HotelsController> _logger;
    public HotelsController(ISender sender, ILogger<HotelsController> logger)
    {
        _sender = sender;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateHotelRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateHotelCommand(
            request.Name,
            request.Description,
            request.Address is not null
                ? new CreateHotelAddressDto(
                    request.Address.Street,
                    request.Address.City,
                    request.Address.Country,
                    request.Address.PostalCode,
                    request.Address.Latitude,
                    request.Address.Longitude)
                : null,
            request.Pictures.Select(p => new CreateHotelPictureDto(
                p.StoredFileId,
                p.AltText,
                p.IsMain)).ToList(),
            request.Amenities?.Select(a => new CreateHotelAmenityDto(
                a.AmenityDefinitionId,
                a.UpchargeType,
                a.UpchargeAmount,
                a.Currency
            )).ToList())
            ;

        var result = await _sender.Send(command, cancellationToken);

        return result.Match(
            success => CreatedAtAction(
                nameof(GetById), 
                new { id = success.Id }, 
                success),
            errors => Problem(
                statusCode: GetStatusCode(errors.First()),
                title: errors.First().Code,
                detail: errors.First().Description));
    }

    [HttpGet]
    public async Task<IActionResult> GetAllHotelSummary(
        [FromQuery] GetAllHotelSummaryFilter filter, 
        CancellationToken cancellationToken)
    {
        var query = new GetAllHotelSummaryQuery(filter);
        var result = await _sender.Send(query, cancellationToken);
        
        return ToApiResponse(result);
    }

    [HttpGet("my-hotels")]
    [Authorize(Policy = "HotelOwner")]
    public async Task<IActionResult> GetMyHotels(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetHotelsByOwnerQuery(), cancellationToken);
        return ToApiResponse(result);
    }

    [HttpPut("{hotelId:guid}")]
    [Authorize(Policy = "HotelOwner")]
    public async Task<IActionResult> UpdateHotelAmenities(Guid hotelId, [FromBody] List<Guid> amenitiesId, CancellationToken cancellationToken)
    {
        var command = new UpdateHotelAmenitiesCommand(hotelId, amenitiesId);
        var result = await _sender.Send(command, cancellationToken);
        return ToApiResponse(result);
    }
    
    [HttpGet("{id:guid}")]
    [AllowAnonymous] 
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetHotelByIdQuery(id), cancellationToken);

        return result.Match<IActionResult>(
            Ok,
            errors => errors.First().Type == ErrorType.NotFound
                ? NotFound(errors.First().Description)
                : BadRequest(errors.First().Description));
    }

    private static int GetStatusCode(Error error) => error.Type switch
    {
        ErrorType.Validation => StatusCodes.Status400BadRequest,
        ErrorType.NotFound => StatusCodes.Status404NotFound,
        ErrorType.Conflict => StatusCodes.Status409Conflict,
        ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
        ErrorType.Forbidden => StatusCodes.Status403Forbidden,
        _ => StatusCodes.Status500InternalServerError
    };
}
