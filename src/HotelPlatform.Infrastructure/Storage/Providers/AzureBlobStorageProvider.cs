// Infrastructure/Storage/Providers/AzureBlobStorageProvider.cs
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using ErrorOr;
using HotelPlatform.Application.Common.Errors;
using HotelPlatform.Application.Common.Interfaces.Storage;
using HotelPlatform.Application.Common.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HotelPlatform.Infrastructure.Storage.Providers;

public class AzureBlobStorageProvider : IStorageProvider
{
    private readonly AzureBlobSettings _settings;
    private readonly ILogger<AzureBlobStorageProvider> _logger;
    private readonly BlobServiceClient? _blobServiceClient;
    private BlobContainerClient? _containerClient;

    public string ProviderName => "AzureBlob";

    public AzureBlobStorageProvider(
        IOptions<StorageSettings> options,
        ILogger<AzureBlobStorageProvider> logger)
    {
        _settings = options.Value.AzureBlob;
        _logger = logger;

        if (!string.IsNullOrEmpty(_settings.ConnectionString))
        {
            try
            {
                _blobServiceClient = new BlobServiceClient(_settings.ConnectionString);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to initialize Azure Blob Storage client");
            }
        }
    }

    public async Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default)
    {
        if (_blobServiceClient is null)
        {
            _logger.LogWarning("Azure Blob Storage client is null - no connection string configured");
            return false;
        }

        try
        {
            await EnsureContainerExistsAsync(cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, 
                "Azure Blob Storage is not available. Exception: {Message}", 
                ex.Message);
            return false;
        }
    }

    public async Task<ErrorOr<StorageUploadResult>> UploadAsync(
        Stream fileStream,
        string fileName,
        string contentType,
        string containerPath,
        CancellationToken cancellationToken = default)
    {
        if (_blobServiceClient is null)
        {
            return StorageErrors.ProviderUnavailable(ProviderName);
        }

        try
        {
            await EnsureContainerExistsAsync(cancellationToken);

            var storedFileName = GenerateStoredFileName(fileName);
            var blobPath = string.IsNullOrEmpty(containerPath)
                ? storedFileName
                : $"{containerPath.TrimEnd('/')}/{storedFileName}";

            var blobClient = _containerClient!.GetBlobClient(blobPath);

            var options = new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders
                {
                    ContentType = contentType
                }
            };

            await blobClient.UploadAsync(fileStream, options, cancellationToken);

            var url = !string.IsNullOrEmpty(_settings.BaseUrl)
                ? $"{_settings.BaseUrl.TrimEnd('/')}/{blobPath}"
                : blobClient.Uri.ToString();

            _logger.LogInformation(
                "Successfully uploaded file {FileName} to Azure Blob Storage at {BlobPath}",
                fileName, blobPath);

            return new StorageUploadResult(url, storedFileName, blobPath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upload file {FileName} to Azure Blob Storage", fileName);
            return StorageErrors.UploadFailed(ex.Message);
        }
    }

    public async Task<ErrorOr<Stream>> DownloadAsync(
        string blobPath,
        CancellationToken cancellationToken = default)
    {
        if (_blobServiceClient is null)
        {
            return StorageErrors.ProviderUnavailable(ProviderName);
        }

        try
        {
            await EnsureContainerExistsAsync(cancellationToken);

            var blobClient = _containerClient!.GetBlobClient(blobPath);

            if (!await blobClient.ExistsAsync(cancellationToken))
            {
                return StorageErrors.FileNotFound;
            }

            var response = await blobClient.DownloadStreamingAsync(cancellationToken: cancellationToken);
            return response.Value.Content;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to download file {BlobPath} from Azure Blob Storage", blobPath);
            return StorageErrors.DownloadFailed(ex.Message);
        }
    }

    public async Task<ErrorOr<Deleted>> DeleteAsync(
        string blobPath,
        CancellationToken cancellationToken = default)
    {
        if (_blobServiceClient is null)
        {
            return StorageErrors.ProviderUnavailable(ProviderName);
        }

        try
        {
            await EnsureContainerExistsAsync(cancellationToken);

            var blobClient = _containerClient!.GetBlobClient(blobPath);
            var response = await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);

            if (!response.Value)
            {
                _logger.LogWarning("File {BlobPath} was not found in Azure Blob Storage", blobPath);
            }

            return Result.Deleted;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete file {BlobPath} from Azure Blob Storage", blobPath);
            return StorageErrors.DeleteFailed(ex.Message);
        }
    }

    private async Task EnsureContainerExistsAsync(CancellationToken cancellationToken)
    {
        if (_containerClient is not null)
            return;

        _containerClient = _blobServiceClient!.GetBlobContainerClient(_settings.ContainerName);

        if (_settings.CreateContainerIfNotExists)
        {
            await _containerClient.CreateIfNotExistsAsync(
                PublicAccessType.None,
                cancellationToken: cancellationToken);
        }
    }

    private static string GenerateStoredFileName(string originalFileName)
    {
        var extension = Path.GetExtension(originalFileName);
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var uniqueId = Guid.NewGuid().ToString("N")[..8];
        return $"{timestamp}_{uniqueId}{extension}";
    }
}