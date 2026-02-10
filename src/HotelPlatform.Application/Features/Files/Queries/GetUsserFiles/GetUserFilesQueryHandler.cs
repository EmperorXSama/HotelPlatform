// Application/Files/Queries/GetUserFiles/GetUserFilesQueryHandler.cs

using HotelPlatform.Application.Common.Interfaces;
using HotelPlatform.Application.Common.Interfaces.Repositories;
using HotelPlatform.Application.Common.Interfaces.Storage;

namespace HotelPlatform.Application.Features.Files.Queries.GetUsserFiles;

public class GetUserFilesQueryHandler
    : IRequestHandler<GetUserFilesQuery, ErrorOr<List<UserFileResult>>>
{
    private readonly IFileStorageService _storageService;
    private readonly ICurrentUser _currentUser;
    private readonly IUserRepository _userRepository;
    private readonly IFileUrlResolver _fileUrlResolver;

    public GetUserFilesQueryHandler(
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

    public async Task<ErrorOr<List<UserFileResult>>> Handle(
        GetUserFilesQuery request,
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

        var result = await _storageService.GetByOwnerAsync(user.Id, cancellationToken);

        return result.Match<ErrorOr<List<UserFileResult>>>(
            files => files.Select(f => new UserFileResult(
                f.Id.Value,
                f.OriginalFileName,
                _fileUrlResolver.GetAccessUrl(f.Id),
                f.ContentType,
                f.SizeInBytes,
                f.CreatedAt)).ToList(),
            errors => errors);
    }
}