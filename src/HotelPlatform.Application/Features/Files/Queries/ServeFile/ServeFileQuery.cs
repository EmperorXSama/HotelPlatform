namespace HotelPlatform.Application.Features.Files.Queries.ServeFile;

public sealed record ServeFileQuery(Guid FileId) 
    : IPublicQuery<ErrorOr<FileDownloadResult>>;