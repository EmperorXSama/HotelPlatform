// Http/IApiClient.cs

using HotelManagement.BlazorServer.Models.Pagination;

namespace HotelManagement.BlazorServer.Http;

public interface IApiClient
{
    // GET methods
    Task<ErrorOr<TResponse>> GetAsync<TResponse>(
        string endpoint,
        CancellationToken cancellationToken = default);

    Task<ErrorOr<TResponse>> GetAsync<TResponse>(
        string endpoint,
        Dictionary<string, string>? queryParams,
        CancellationToken cancellationToken = default);

    Task<ErrorOr<PagedResult<TResponse>>> GetPagedAsync<TResponse>(
        string endpoint,
        PagedRequest request,
        CancellationToken cancellationToken = default) where TResponse : class;

    // POST methods
    Task<ErrorOr<TResponse>> PostAsync<TRequest, TResponse>(
        string endpoint,
        TRequest request,
        CancellationToken cancellationToken = default);

    Task<ErrorOr<Success>> PostAsync<TRequest>(
        string endpoint,
        TRequest request,
        CancellationToken cancellationToken = default);

    Task<ErrorOr<TResponse>> PostMultipartAsync<TResponse>(
        string endpoint,
        MultipartFormDataContent content,
        CancellationToken cancellationToken = default);

    // PUT methods
    Task<ErrorOr<TResponse>> PutAsync<TRequest, TResponse>(
        string endpoint,
        TRequest request,
        CancellationToken cancellationToken = default);

    Task<ErrorOr<Success>> PutAsync<TRequest>(
        string endpoint,
        TRequest request,
        CancellationToken cancellationToken = default);

    // PATCH methods
    Task<ErrorOr<TResponse>> PatchAsync<TRequest, TResponse>(
        string endpoint,
        TRequest request,
        CancellationToken cancellationToken = default);

    // DELETE methods
    Task<ErrorOr<Success>> DeleteAsync(
        string endpoint,
        CancellationToken cancellationToken = default);
}