// Models/Responses/Files/FileUploadResponse.cs

namespace HotelManagement.BlazorServer.models.Response.Files;

public sealed record FileUploadResponse(
    Guid Id,
    string FileName,
    string Url,
    string ContentType,
    long SizeInBytes)
{
    public string FormattedSize => FormatFileSize(SizeInBytes);

    private static string FormatFileSize(long bytes)
    {
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