// Application/Common/Interfaces/Storage/IFileStorageService.cs

using HotelPlatform.Domain.Files;

namespace HotelPlatform.Application.Common.Interfaces;

public interface IFileStorageService
{
    Task<ErrorOr<StoredFile>> UploadAsync(
        Stream fileStream,
        string fileName,
        string contentType,
        UserId ownerId,
        CancellationToken cancellationToken = default);

    Task<ErrorOr<Stream>> DownloadAsync(
        StoredFileId fileId,
        CancellationToken cancellationToken = default);

    Task<ErrorOr<Deleted>> DeleteAsync(
        StoredFileId fileId,
        CancellationToken cancellationToken = default);

    Task<ErrorOr<IReadOnlyList<StoredFile>>> GetByOwnerAsync(
        UserId ownerId,
        CancellationToken cancellationToken = default);
}