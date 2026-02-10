namespace HotelManagement.BlazorServer.Models;

/// <summary>
/// Represents an uploaded picture ready to be attached to a hotel or room
/// </summary>
public class UploadedPictureModel
{
    public Guid StoredFileId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long SizeInBytes { get; set; }
    public string? AltText { get; set; }
    public bool IsMain { get; set; }
    public int DisplayOrder { get; set; }
    
    /// <summary>
    /// Upload progress (0-100), null when complete
    /// </summary>
    public int? UploadProgress { get; set; }
    
    /// <summary>
    /// True while file is being uploaded
    /// </summary>
    public bool IsUploading { get; set; }
    
    /// <summary>
    /// Error message if upload failed
    /// </summary>
    public string? ErrorMessage { get; set; }
    
    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);
    
    public string FormattedSize => FormatFileSize(SizeInBytes);

    private static string FormatFileSize(long bytes)
    {
        if (bytes == 0) return "0 B";
        
        string[] sizes = ["B", "KB", "MB", "GB"];
        int order = 0;
        double size = bytes;

        while (size >= 1024 && order < sizes.Length - 1)
        {
            order++;
            size /= 1024;
        }

        return $"{size:0.##} {sizes[order]}";
    }
}

/// <summary>
/// Configuration for the picture uploader component
/// </summary>
public class PictureUploaderConfig
{
    /// <summary>
    /// Maximum number of pictures allowed (0 = unlimited)
    /// </summary>
    public int MaxFiles { get; set; } = 10;
    
    /// <summary>
    /// Maximum file size in bytes (default 5MB)
    /// </summary>
    public long MaxFileSizeBytes { get; set; } = 5 * 1024 * 1024;
    
    /// <summary>
    /// Allowed content types
    /// </summary>
    public List<string> AllowedContentTypes { get; set; } = 
    [
        "image/jpeg",
        "image/png",
        "image/webp",
        "image/gif"
    ];
    
    /// <summary>
    /// Allowed file extensions for display
    /// </summary>
    public string AllowedExtensions => "JPG, PNG, WebP, GIF";
    
    /// <summary>
    /// Whether to require at least one picture
    /// </summary>
    public bool RequireAtLeastOne { get; set; } = true;
    
    /// <summary>
    /// Title displayed on the component
    /// </summary>
    public string Title { get; set; } = "Hotel Pictures";
    
    /// <summary>
    /// Subtitle/helper text
    /// </summary>
    public string Subtitle { get; set; } = "Upload images of your property. The first image will be the main photo.";
}
