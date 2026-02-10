

namespace HotelPlatform.Application.Features.Files.Commands.UploadFileCommand;

public sealed record UploadFileCommand(
    Stream FileStream,
    string FileName,
    string ContentType) : ICommand<ErrorOr<UploadFileResult>>;

public sealed record UploadFileResult(
    Guid Id,
    string FileName,
    string Url,
    string ContentType,
    long SizeInBytes);