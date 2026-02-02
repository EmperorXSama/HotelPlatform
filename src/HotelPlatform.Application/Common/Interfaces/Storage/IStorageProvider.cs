// Application/Common/Interfaces/Storage/IStorageProvider.cs

namespace HotelPlatform.Application.Common.Interfaces.Storage;

public interface IStorageProvider
{
    string ProviderName { get; }
    
    Task<ErrorOr<StorageUploadResult>> UploadAsync(
        Stream fileStream,
        string fileName,
        string contentType,
        string containerPath,
        CancellationToken cancellationToken = default);

    Task<ErrorOr<Stream>> DownloadAsync(
        string blobPath,
        CancellationToken cancellationToken = default);

    Task<ErrorOr<Deleted>> DeleteAsync(
        string blobPath,
        CancellationToken cancellationToken = default);

    Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default);
}

public sealed record StorageUploadResult(
    string Url,
    string StoredFileName,
    string BlobPath);