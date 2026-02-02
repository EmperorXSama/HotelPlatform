// Api/Controllers/HotelsController.cs

using ErrorOr;
using HotelPlatform.Application.Features.Hotels.Commands.CreateHotel;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelPlatform.Api.Controller;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Tags("Hotels")]
public class HotelsController : ControllerBase
{
    private readonly ISender _sender;

    public HotelsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateHotelResult), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Create(
        [FromBody] CreateHotelRequest request,
        CancellationToken cancellationToken)
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
                p.IsMain)).ToList());

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

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        // TODO: Implement GetHotelByIdQuery
        return Ok();
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

public sealed record CreateHotelRequest(
    string Name,
    string? Description,
    CreateHotelAddressRequest? Address,
    List<CreateHotelPictureRequest> Pictures);

public sealed record CreateHotelAddressRequest(
    string Street,
    string City,
    string Country,
    string? PostalCode,
    double? Latitude,
    double? Longitude);

public sealed record CreateHotelPictureRequest(
    Guid StoredFileId,
    string? AltText,
    bool IsMain);