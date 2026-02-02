// Infrastructure/Storage/Providers/LocalFileStorageProvider.cs
using ErrorOr;
using HotelPlatform.Application.Common.Errors;
using HotelPlatform.Application.Common.Interfaces.Storage;
using HotelPlatform.Application.Common.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HotelPlatform.Infrastructure.Storage.Providers;

public class LocalFileStorageProvider : IStorageProvider
{
    private readonly LocalStorageSettings _settings;
    private readonly ILogger<LocalFileStorageProvider> _logger;
    private readonly string _basePath;

    public string ProviderName => "LocalFile";

    public LocalFileStorageProvider(
        IOptions<StorageSettings> options,
        ILogger<LocalFileStorageProvider> logger)
    {
        _settings = options.Value.LocalStorage;
        _logger = logger;
        
        // Resolve the base path relative to content root or use absolute path
        _basePath = Path.IsPathRooted(_settings.BasePath)
            ? _settings.BasePath
            : Path.Combine(Directory.GetCurrentDirectory(), _settings.BasePath);
    }

    public Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // Ensure directory exists and is writable
            if (!Directory.Exists(_basePath))
            {
                Directory.CreateDirectory(_basePath);
            }

            // Test write access
            var testFile = Path.Combine(_basePath, ".write_test");
            File.WriteAllText(testFile, "test");
            File.Delete(testFile);

            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Local file storage is not available at {BasePath}", _basePath);
            return Task.FromResult(false);
        }
    }

    public async Task<ErrorOr<StorageUploadResult>> UploadAsync(
        Stream fileStream,
        string fileName,
        string contentType,
        string containerPath,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var storedFileName = GenerateStoredFileName(fileName);
            
            // Build the directory path
            var directoryPath = string.IsNullOrEmpty(containerPath)
                ? _basePath
                : Path.Combine(_basePath, containerPath);

            // Ensure directory exists
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            var filePath = Path.Combine(directoryPath, storedFileName);
            var relativePath = Path.GetRelativePath(_basePath, filePath).Replace("\\", "/");

            // Save file
            await using var fileStreamOut = new FileStream(
                filePath,
                FileMode.Create,
                FileAccess.Write,
                FileShare.None,
                bufferSize: 81920,
                useAsync: true);

            await fileStream.CopyToAsync(fileStreamOut, cancellationToken);

            var url = $"{_settings.BaseUrl.TrimEnd('/')}/{relativePath}";

            _logger.LogInformation(
                "Successfully uploaded file {FileName} to local storage at {FilePath}",
                fileName, filePath);

            return new StorageUploadResult(url, storedFileName, relativePath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upload file {FileName} to local storage", fileName);
            return StorageErrors.UploadFailed(ex.Message);
        }
    }

    public Task<ErrorOr<Stream>> DownloadAsync(
        string blobPath,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var filePath = Path.Combine(_basePath, blobPath);

            if (!File.Exists(filePath))
            {
                return Task.FromResult<ErrorOr<Stream>>(StorageErrors.FileNotFound);
            }

            var stream = new FileStream(
                filePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                bufferSize: 81920,
                useAsync: true);

            return Task.FromResult<ErrorOr<Stream>>(stream);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to download file {BlobPath} from local storage", blobPath);
            return Task.FromResult<ErrorOr<Stream>>(StorageErrors.DownloadFailed(ex.Message));
        }
    }

    public Task<ErrorOr<Deleted>> DeleteAsync(
        string blobPath,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var filePath = Path.Combine(_basePath, blobPath);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                _logger.LogInformation("Deleted file {FilePath} from local storage", filePath);
            }
            else
            {
                _logger.LogWarning("File {FilePath} was not found in local storage", filePath);
            }

            return Task.FromResult<ErrorOr<Deleted>>(Result.Deleted);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete file {BlobPath} from local storage", blobPath);
            return Task.FromResult<ErrorOr<Deleted>>(StorageErrors.DeleteFailed(ex.Message));
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