// Services/Files/FileApiService.cs

using HotelManagement.BlazorServer.Http;
using HotelManagement.BlazorServer.models.Response.Files;

namespace HotelManagement.BlazorServer.Services.Files;

public sealed class FileApiService : IFileApiService
{
    private readonly IApiClient _apiClient;
    private const string BaseEndpoint = "api/files";

    public FileApiService(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<ErrorOr<FileUploadResponse>> UploadAsync(
        Stream fileStream,
        string fileName,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        using var content = new MultipartFormDataContent();
        using var streamContent = new StreamContent(fileStream);

        streamContent.Headers.ContentType =
            new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);

        content.Add(streamContent, "file", fileName);

        return await _apiClient.PostMultipartAsync<FileUploadResponse>(
            $"{BaseEndpoint}/upload",
            content,
            cancellationToken);
    }

    public async Task<ErrorOr<Success>> DeleteAsync(
        Guid fileId,
        CancellationToken cancellationToken = default)
    {
        return await _apiClient.DeleteAsync(
            $"{BaseEndpoint}/{fileId}",
            cancellationToken);
    }
}