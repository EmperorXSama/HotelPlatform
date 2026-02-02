// Application/Common/Settings/StorageSettings.cs
using System.ComponentModel.DataAnnotations;

namespace HotelPlatform.Application.Common.Settings;

public class StorageSettings
{
    public const string SectionName = "Storage";

    [Required]
    public AzureBlobSettings AzureBlob { get; set; } = new();
    
    [Required]
    public LocalStorageSettings LocalStorage { get; set; } = new();
    
    public FileValidationSettings Validation { get; set; } = new();
    
    /// <summary>
    /// If true, tries Azure Blob first, falls back to local storage on failure.
    /// If false, uses only the primary provider.
    /// </summary>
    public bool EnableFallback { get; set; } = true;
    
    /// <summary>
    /// Primary storage provider: "AzureBlob" or "LocalFile"
    /// </summary>
    public string PrimaryProvider { get; set; } = "AzureBlob";
}
// Application/Common/Settings/StorageSettings.cs
public class AzureBlobSettings
{
    public string ConnectionString { get; set; } = string.Empty;
    public string ContainerName { get; set; } = "hotel-files";
    public string? BaseUrl { get; set; }
    public bool CreateContainerIfNotExists { get; set; } = true;
}

public class LocalStorageSettings
{
    /// <summary>
    /// Base path for storing files locally
    /// </summary>
    public string BasePath { get; set; } = "uploads";
    
    /// <summary>
    /// Base URL for accessing local files
    /// </summary>
    public string BaseUrl { get; set; } = "/files";
}

public class FileValidationSettings
{
    /// <summary>
    /// Maximum file size in bytes (default 10MB)
    /// </summary>
    public long MaxFileSizeBytes { get; set; } = 10 * 1024 * 1024;
    
    /// <summary>
    /// Allowed content types
    /// </summary>
    public string[] AllowedContentTypes { get; set; } = 
    {
        "image/jpeg",
        "image/png",
        "image/gif",
        "image/webp"
    };
    
    /// <summary>
    /// Allowed file extensions
    /// </summary>
    public string[] AllowedExtensions { get; set; } = 
    {
        ".jpg",
        ".jpeg",
        ".png",
        ".gif",
        ".webp"
    };
}