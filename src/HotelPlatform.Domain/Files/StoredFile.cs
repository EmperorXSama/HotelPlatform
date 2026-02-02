
using HotelPlatform.Domain.Common.StronglyTypedIds;

namespace HotelPlatform.Domain.Files;

public class StoredFile : AggregateRoot<StoredFileId>
{
    public UserId OwnerId { get; private set; }
    public string OriginalFileName { get; private set; } = string.Empty;
    public string StoredFileName { get; private set; } = string.Empty;
    public string ContentType { get; private set; } = string.Empty;
    public long SizeInBytes { get; private set; }
    public string Url { get; private set; } = string.Empty;
    public string StorageProvider { get; private set; } = string.Empty; // "AzureBlob" or "LocalFile"
    public string? BlobPath { get; private set; } // Full path in storage

    private StoredFile() : base() { }

    private StoredFile(
        StoredFileId id,
        UserId ownerId,
        string originalFileName,
        string storedFileName,
        string contentType,
        long sizeInBytes,
        string url,
        string storageProvider,
        string? blobPath) : base(id)
    {
        OwnerId = ownerId;
        OriginalFileName = originalFileName;
        StoredFileName = storedFileName;
        ContentType = contentType;
        SizeInBytes = sizeInBytes;
        Url = url;
        StorageProvider = storageProvider;
        BlobPath = blobPath;
    }

    public static StoredFile Create(
        UserId ownerId,
        string originalFileName,
        string storedFileName,
        string contentType,
        long sizeInBytes,
        string url,
        string storageProvider,
        string? blobPath = null)
    {
        return new StoredFile(
            StoredFileId.New(),
            ownerId,
            originalFileName,
            storedFileName,
            contentType,
            sizeInBytes,
            url,
            storageProvider,
            blobPath);
    }
}