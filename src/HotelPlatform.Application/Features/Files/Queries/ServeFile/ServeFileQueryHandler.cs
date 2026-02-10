using HotelPlatform.Application.Common.Interfaces;

namespace HotelPlatform.Application.Features.Files.Queries.ServeFile;


public class ServeFileQueryHandler 
    : IRequestHandler<ServeFileQuery, ErrorOr<FileDownloadResult>>
{
    private readonly IFileStorageService _storageService;

    public ServeFileQueryHandler(IFileStorageService storageService)
    {
        _storageService = storageService;
    }

    public async Task<ErrorOr<FileDownloadResult>> Handle(
        ServeFileQuery request,
        CancellationToken cancellationToken)
    {
        StoredFileId fileId = (StoredFileId)request.FileId;
        return await _storageService.GetFileStreamAsync(fileId, cancellationToken);
    }
}