namespace HotelPlatform.Application.Features.Files.Queries.GetUsserFiles;

public sealed record GetUserFilesQuery : IQuery<ErrorOr<List<UserFileResult>>>;

public sealed record UserFileResult(
    Guid Id,
    string FileName,
    string Url,
    string ContentType,
    long SizeInBytes,
    DateTimeOffset UploadedAt);