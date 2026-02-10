// Infrastructure/Storage/FileStorageService.cs
using ErrorOr;
using HotelPlatform.Application.Common.Errors;
using HotelPlatform.Application.Common.Interfaces.Repositories;
using HotelPlatform.Application.Common.Interfaces.Storage;
using HotelPlatform.Application.Common.Interfaces;
using HotelPlatform.Application.Common.Settings;
using HotelPlatform.Application.Features.Files;
using HotelPlatform.Domain.Common.StronglyTypedIds;
using HotelPlatform.Domain.Files;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HotelPlatform.Infrastructure.Storage;

public class FileStorageService : IFileStorageService
{
    private readonly IStorageProvider _primaryProvider;
    private readonly IStorageProvider _fallbackProvider;
    private readonly IStoredFileRepository _storedFileRepository;
    private readonly IFileValidator _fileValidator;
    private readonly StorageSettings _settings;
    private readonly ILogger<FileStorageService> _logger;

    public FileStorageService(
        IEnumerable<IStorageProvider> providers,
        IStoredFileRepository storedFileRepository,
        IFileValidator fileValidator,
        IOptions<StorageSettings> options,
        ILogger<FileStorageService> logger)
    {
        _storedFileRepository = storedFileRepository;
        _fileValidator = fileValidator;
        _settings = options.Value;
        _logger = logger;

        var providerList = providers.ToList();
        
        // Determine primary and fallback providers
        _primaryProvider = _settings.PrimaryProvider.Equals("AzureBlob", StringComparison.OrdinalIgnoreCase)
            ? providerList.First(p => p.ProviderName == "AzureBlob")
            : providerList.First(p => p.ProviderName == "LocalFile");

        _fallbackProvider = _primaryProvider.ProviderName == "AzureBlob"
            ? providerList.First(p => p.ProviderName == "LocalFile")
            : providerList.First(p => p.ProviderName == "AzureBlob");
    }

    public async Task<ErrorOr<StoredFile>> UploadAsync(
        Stream fileStream,
        string fileName,
        string contentType,
        UserId ownerId,
        CancellationToken cancellationToken = default)
    {
        
        // Validate file
        var validationResult = _fileValidator.Validate(fileName, contentType, fileStream.Length);
        if (validationResult.IsError)
            return validationResult.Errors;

        var fileSize = fileStream.Length;
        
        // Generate container path based on owner and date
        var containerPath = $"{ownerId.Value:N}/{DateTime.UtcNow:yyyy/MM}";

        // Try primary provider
        var uploadResult = await TryUploadAsync(
            _primaryProvider,
            fileStream,
            fileName,
            contentType,
            containerPath,
            cancellationToken);

        // Fallback if enabled and primary failed
        if (uploadResult.IsError && _settings.EnableFallback)
        {
            _logger.LogWarning(
                "Primary storage provider {Provider} failed, falling back to {FallbackProvider}",
                _primaryProvider.ProviderName,
                _fallbackProvider.ProviderName);

            // Reset stream position
            if (fileStream.CanSeek)
            {
                fileStream.Position = 0;
            }

            uploadResult = await TryUploadAsync(
                _fallbackProvider,
                fileStream,
                fileName,
                contentType,
                containerPath,
                cancellationToken);
        }

        if (uploadResult.IsError)
        {
            return uploadResult.Errors;
        }

        var (url, storedFileName, blobPath, providerName) = uploadResult.Value;

        // Create StoredFile entity
        var storedFile = StoredFile.Create(
            ownerId,
            fileName,
            storedFileName,
            contentType,
            fileSize,
            url,
            providerName,
            blobPath);
        
        await _storedFileRepository.AddAsync(storedFile, cancellationToken);
        _logger.LogInformation(
            "File {FileName} uploaded successfully to {Provider}. StoredFileId: {StoredFileId}",
            fileName, providerName, storedFile.Id);

        return storedFile;
    }

    public async Task<ErrorOr<Stream>> DownloadAsync(
        StoredFileId fileId,
        CancellationToken cancellationToken = default)
    {
        var storedFile = await _storedFileRepository.GetByIdAsync(fileId, cancellationToken);
        if (storedFile is null)
        {
            return StorageErrors.FileNotFound;
        }

        var provider = GetProviderByName(storedFile.StorageProvider);
        if (provider is null)
        {
            return StorageErrors.ProviderUnavailable(storedFile.StorageProvider);
        }

        return await provider.DownloadAsync(storedFile.BlobPath!, cancellationToken);
    }

    public async Task<ErrorOr<Deleted>> DeleteAsync(
        StoredFileId fileId,
        CancellationToken cancellationToken = default)
    {
        var storedFile = await _storedFileRepository.GetByIdAsync(fileId, cancellationToken);
        if (storedFile is null)
        {
            return StorageErrors.FileNotFound;
        }

        // Check if file is in use
        var isInUse = await _storedFileRepository.IsInUseAsync(fileId, cancellationToken);
        if (isInUse)
        {
            return Error.Conflict(
                code: "Storage.FileInUse",
                description: "Cannot delete file because it is still in use.");
        }

        var provider = GetProviderByName(storedFile.StorageProvider);
        if (provider is not null && !string.IsNullOrEmpty(storedFile.BlobPath))
        {
            var deleteResult = await provider.DeleteAsync(storedFile.BlobPath, cancellationToken);
            if (deleteResult.IsError)
            {
                _logger.LogWarning(
                    "Failed to delete file from storage provider, but will remove database record. Error: {Error}",
                    deleteResult.FirstError.Description);
            }
        }

        _storedFileRepository.Delete(storedFile);
        return Result.Deleted;
    }

    public async Task<ErrorOr<IReadOnlyList<StoredFile>>> GetByOwnerAsync(
        UserId ownerId,
        CancellationToken cancellationToken = default)
    {
        var files = await _storedFileRepository.GetByOwnerIdAsync(ownerId, cancellationToken);
        return files.ToList();
    }

    public async Task<ErrorOr<FileDownloadResult>> GetFileStreamAsync(
        StoredFileId fileId,
        CancellationToken cancellationToken = default)
    {
        var storedFile = await _storedFileRepository.GetByIdAsync(fileId, cancellationToken);
        if (storedFile is null)
            return StorageErrors.FileNotFound;

        var provider = GetProviderByName(storedFile.StorageProvider);
        if (provider is null)
            return StorageErrors.ProviderUnavailable(storedFile.StorageProvider);

        var streamResult = await provider.DownloadAsync(storedFile.BlobPath!, cancellationToken);
        if (streamResult.IsError)
            return streamResult.Errors;

        return new FileDownloadResult(
            streamResult.Value,
            storedFile.ContentType,
            storedFile.OriginalFileName);
    }

    private async Task<ErrorOr<(string Url, string StoredFileName, string BlobPath, string ProviderName)>> TryUploadAsync(
        IStorageProvider provider,
        Stream fileStream,
        string fileName,
        string contentType,
        string containerPath,
        CancellationToken cancellationToken)
    {
        if (!await provider.IsAvailableAsync(cancellationToken))
        {
            return StorageErrors.ProviderUnavailable(provider.ProviderName);
        }

        var result = await provider.UploadAsync(
            fileStream,
            fileName,
            contentType,
            containerPath,
            cancellationToken);

        if (result.IsError)
            return result.Errors;

        return (result.Value.Url, result.Value.StoredFileName, result.Value.BlobPath, provider.ProviderName);
    }

    private IStorageProvider? GetProviderByName(string providerName)
    {
        if (_primaryProvider.ProviderName.Equals(providerName, StringComparison.OrdinalIgnoreCase))
            return _primaryProvider;

        if (_fallbackProvider.ProviderName.Equals(providerName, StringComparison.OrdinalIgnoreCase))
            return _fallbackProvider;

        return null;
    }
}