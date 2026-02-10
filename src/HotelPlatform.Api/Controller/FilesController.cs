using ErrorOr;
using HotelPlatform.Api.Controller;
using HotelPlatform.Application.Features.Files.Commands.UploadFileCommand;
using HotelPlatform.Application.Features.Files.Queries.GetUsserFiles;
using HotelPlatform.Application.Features.Files.Queries.ServeFile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[Authorize]
[Tags("Files")]
public class FilesController : ApiBaseController  // Change this
{
    private readonly ISender _sender;

    public FilesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> ServeFile(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new ServeFileQuery(id), cancellationToken);

        // Keep this as-is since it returns a file stream, not JSON
        return result.Match<IActionResult>(
            file => File(file.Stream, file.ContentType, file.FileName, enableRangeProcessing: true),
            errors => errors.First().Type == ErrorType.NotFound
                ? NotFound()
                : BadRequest(errors.First().Description));
    }

    [HttpPost("upload")]
    [DisableRequestSizeLimit]
    public async Task<IActionResult> UploadFile(
        IFormFile file,
        CancellationToken cancellationToken)
    {
        await using var stream = file.OpenReadStream();

        var result = await _sender.Send(
            new UploadFileCommand(stream, file.FileName, file.ContentType),
            cancellationToken);

        return ToApiResponse(result);  // Use wrapped response
    }

    [HttpGet("my-files")]
    public async Task<IActionResult> GetUserFiles(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetUserFilesQuery(), cancellationToken);

        return ToApiResponse(result);  // Use wrapped response
    }
}