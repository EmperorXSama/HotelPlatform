namespace HotelPlatform.Application.Features.Files;

public sealed record FileDownloadResult(
    Stream Stream,
    string ContentType,
    string FileName);