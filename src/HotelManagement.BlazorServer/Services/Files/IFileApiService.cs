// Services/Files/IFileApiService.cs

using HotelManagement.BlazorServer.models.Response.Files;

namespace HotelManagement.BlazorServer.Services.Files;

public interface IFileApiService
{
    Task<ErrorOr<FileUploadResponse>> UploadAsync(
        Stream fileStream,
        string fileName,
        string contentType,
        CancellationToken cancellationToken = default);

    Task<ErrorOr<Success>> DeleteAsync(
        Guid fileId,
        CancellationToken cancellationToken = default);
}