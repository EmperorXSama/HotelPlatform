// Api/Controllers/FilesController.cs

using HotelPlatform.Application.Common.Interfaces;
using HotelPlatform.Application.Common.Interfaces.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelPlatform.Api.Controller;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Tags("Files")]
public class FilesController : ControllerBase
{
    private readonly IFileStorageService _storageService;
    private readonly ICurrentUser _currentUser;
    private readonly IUserRepository _userRepository;

    public FilesController(
        IFileStorageService storageService,
        ICurrentUser currentUser,
        IUserRepository userRepository)
    {
        _storageService = storageService;
        _currentUser = currentUser;
        _userRepository = userRepository;
    }

    /// <summary>
    /// Upload a file
    /// </summary>
    [HttpPost("upload")]
    [DisableRequestSizeLimit]
    [ProducesResponseType(typeof(UploadFileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UploadFile(
        IFormFile file,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated || string.IsNullOrEmpty(_currentUser.Id))
        {
            return Unauthorized();
        }

        var user = await _userRepository.GetByIdentityIdAsync(_currentUser.Id, cancellationToken);
        if (user is null)
        {
            return NotFound("User profile not found.");
        }

        await using var stream = file.OpenReadStream();
        var result = await _storageService.UploadAsync(
            stream,
            file.FileName,
            file.ContentType,
            user.Id,
            cancellationToken);

        return result.Match<IActionResult>(
            storedFile => Ok(new UploadFileResponse(
                storedFile.Id.Value,
                storedFile.OriginalFileName,
                storedFile.Url,
                storedFile.ContentType,
                storedFile.SizeInBytes)),
            errors => BadRequest(errors.First().Description));
    }

    /// <summary>
    /// Get all files for the current user
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<FileResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserFiles(CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated || string.IsNullOrEmpty(_currentUser.Id))
        {
            return Unauthorized();
        }

        var user = await _userRepository.GetByIdentityIdAsync(_currentUser.Id, cancellationToken);
        if (user is null)
        {
            return NotFound("User profile not found.");
        }

        var result = await _storageService.GetByOwnerAsync(user.Id, cancellationToken);

        return result.Match<IActionResult>(
            files => Ok(files.Select(f => new FileResponse(
                f.Id.Value,
                f.OriginalFileName,
                f.Url,
                f.ContentType,
                f.SizeInBytes,
                f.CreatedAt))),
            errors => BadRequest(errors.First().Description));
    }
}

public sealed record UploadFileResponse(
    Guid Id,
    string FileName,
    string Url,
    string ContentType,
    long SizeInBytes);

public sealed record FileResponse(
    Guid Id,
    string FileName,
    string Url,
    string ContentType,
    long SizeInBytes,
    DateTimeOffset UploadedAt);