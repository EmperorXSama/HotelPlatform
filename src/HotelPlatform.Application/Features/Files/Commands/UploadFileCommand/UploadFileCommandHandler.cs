
using HotelPlatform.Application.Common.Interfaces;
using HotelPlatform.Application.Common.Interfaces.Repositories;
using HotelPlatform.Application.Common.Interfaces.Storage;

namespace HotelPlatform.Application.Features.Files.Commands.UploadFileCommand;

public class UploadFileCommandHandler
    : IRequestHandler<UploadFileCommand, ErrorOr<UploadFileResult>>
{
    private readonly IFileStorageService _storageService;
    private readonly ICurrentUser _currentUser;
    private readonly IUserRepository _userRepository;
    private readonly IFileUrlResolver _fileUrlResolver;

    public UploadFileCommandHandler(
        IFileStorageService storageService,
        ICurrentUser currentUser,
        IUserRepository userRepository,
        IFileUrlResolver fileUrlResolver)
    {
        _storageService = storageService;
        _currentUser = currentUser;
        _userRepository = userRepository;
        _fileUrlResolver = fileUrlResolver;
    }

    public async Task<ErrorOr<UploadFileResult>> Handle(
        UploadFileCommand request,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated || string.IsNullOrEmpty(_currentUser.Id))
            return Error.Unauthorized(
                code: "File.Unauthorized",
                description: "User is not authenticated.");

        var user = await _userRepository.GetByIdentityIdAsync(
            _currentUser.Id, cancellationToken);

        if (user is null)
            return Error.NotFound(
                code: "File.UserNotFound",
                description: "User profile not found.");

        var result = await _storageService.UploadAsync(
            request.FileStream,
            request.FileName,
            request.ContentType,
            user.Id,
            cancellationToken);

        return result.Match<ErrorOr<UploadFileResult>>(
            storedFile => new UploadFileResult(
                storedFile.Id.Value,
                storedFile.OriginalFileName,
                _fileUrlResolver.GetAccessUrl(storedFile.Id),
                storedFile.ContentType,
                storedFile.SizeInBytes),
            errors => errors);
    }
}