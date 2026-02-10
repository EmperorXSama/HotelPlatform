using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using HotelManagement.BlazorServer.Errors;
using HotelManagement.BlazorServer.Models.Pagination;
using HotelManagement.BlazorServer.models.Response;
using Microsoft.Extensions.Options;

namespace HotelManagement.BlazorServer.Http;

public sealed class ApiClient : IApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ApiClient> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public ApiClient(
        HttpClient httpClient, 
        ILogger<ApiClient> logger, 
        IOptions<JsonSerializerOptions>? jsonOptions)
    {
        _httpClient = httpClient;
        _logger = logger;
        _jsonOptions = jsonOptions?.Value ?? new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) } // ✅ Add this
        };
    }

    #region GET Methods

    public async Task<ErrorOr<TResponse>> GetAsync<TResponse>(
        string endpoint,
        CancellationToken cancellationToken = default)
    {
        return await GetAsync<TResponse>(endpoint, null, cancellationToken);
    }

    public async Task<ErrorOr<TResponse>> GetAsync<TResponse>(
        string endpoint,
        Dictionary<string, string>? queryParams,
        CancellationToken cancellationToken = default)
    {
        var url = BuildUrlWithQueryParams(endpoint, queryParams);
        return await SendAsync<TResponse>(
            () => new HttpRequestMessage(HttpMethod.Get, url),
            cancellationToken);
    }

    public async Task<ErrorOr<PagedResult<TResponse>>> GetPagedAsync<TResponse>(
        string endpoint,
        PagedRequest request,
        CancellationToken cancellationToken = default) where TResponse : class
    {
        var queryParams = request.ToQueryParameters();
        return await GetAsync<PagedResult<TResponse>>(endpoint, queryParams, cancellationToken);
    }

    #endregion

    #region POST Methods

    public async Task<ErrorOr<TResponse>> PostAsync<TRequest, TResponse>(
        string endpoint,
        TRequest request,
        CancellationToken cancellationToken = default)
    {
        return await SendAsync<TResponse>(
            () => CreateJsonRequest(HttpMethod.Post, endpoint, request),
            cancellationToken);
    }

    public async Task<ErrorOr<Success>> PostAsync<TRequest>(
        string endpoint,
        TRequest request,
        CancellationToken cancellationToken = default)
    {
        return await SendWithoutResponseAsync(
            () => CreateJsonRequest(HttpMethod.Post, endpoint, request),
            cancellationToken);
    }

    public async Task<ErrorOr<TResponse>> PostMultipartAsync<TResponse>(
        string endpoint,
        MultipartFormDataContent content,
        CancellationToken cancellationToken = default)
    {
        return await SendAsync<TResponse>(
            () => new HttpRequestMessage(HttpMethod.Post, endpoint) { Content = content },
            cancellationToken);
    }

    #endregion

    #region PUT Methods

    public async Task<ErrorOr<TResponse>> PutAsync<TRequest, TResponse>(
        string endpoint,
        TRequest request,
        CancellationToken cancellationToken = default)
    {
        return await SendAsync<TResponse>(
            () => CreateJsonRequest(HttpMethod.Put, endpoint, request),
            cancellationToken);
    }

    public async Task<ErrorOr<Success>> PutAsync<TRequest>(
        string endpoint,
        TRequest request,
        CancellationToken cancellationToken = default)
    {
        return await SendWithoutResponseAsync(
            () => CreateJsonRequest(HttpMethod.Put, endpoint, request),
            cancellationToken);
    }

    #endregion

    #region PATCH Methods

    public async Task<ErrorOr<TResponse>> PatchAsync<TRequest, TResponse>(
        string endpoint,
        TRequest request,
        CancellationToken cancellationToken = default)
    {
        return await SendAsync<TResponse>(
            () => CreateJsonRequest(HttpMethod.Patch, endpoint, request),
            cancellationToken);
    }

    #endregion

    #region DELETE Methods

    public async Task<ErrorOr<Success>> DeleteAsync(
        string endpoint,
        CancellationToken cancellationToken = default)
    {
        return await SendWithoutResponseAsync(
            () => new HttpRequestMessage(HttpMethod.Delete, endpoint),
            cancellationToken);
    }

    #endregion

    #region Core Send Methods

    /// <summary>
    /// Sends request and expects ApiResponse{T} wrapper with data
    /// </summary>
    private async Task<ErrorOr<TResponse>> SendAsync<TResponse>(
        Func<HttpRequestMessage> requestFactory,
        CancellationToken cancellationToken)
    {
        // Send the HTTP request
        var sendResult = await SendRequestAsync(requestFactory, cancellationToken);
        if (sendResult.IsError)
        {
            return sendResult.Errors;
        }

        var response = sendResult.Value;

        // Handle non-success HTTP status codes that don't have ApiResponse body
        if (!response.IsSuccessStatusCode && IsNonApiErrorStatus(response.StatusCode))
        {
            return HandleNonApiErrorStatus(response.StatusCode);
        }

        // Deserialize the ApiResponse wrapper
        var apiResponse = await DeserializeApiResponseAsync<TResponse>(response, cancellationToken);
        if (apiResponse.IsError)
        {
            return apiResponse.Errors;
        }

        // Unwrap the ApiResponse
        return UnwrapApiResponse(apiResponse.Value);
    }

    /// <summary>
    /// Sends request that doesn't return data (POST without response, DELETE, etc.)
    /// </summary>
    private async Task<ErrorOr<Success>> SendWithoutResponseAsync(
        Func<HttpRequestMessage> requestFactory,
        CancellationToken cancellationToken)
    {
        var sendResult = await SendRequestAsync(requestFactory, cancellationToken);
        if (sendResult.IsError)
        {
            return sendResult.Errors;
        }

        var response = sendResult.Value;

        // NoContent responses don't have a body
        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            return Result.Success;
        }

        // Handle non-success HTTP status codes
        if (!response.IsSuccessStatusCode && IsNonApiErrorStatus(response.StatusCode))
        {
            return HandleNonApiErrorStatus(response.StatusCode);
        }

        // Deserialize the ApiResponse wrapper (without data)
        var apiResponse = await DeserializeApiResponseAsync(response, cancellationToken);
        if (apiResponse.IsError)
        {
            return apiResponse.Errors;
        }

        // Check if API reported success
        if (!apiResponse.Value.IsSuccess)
        {
            return apiResponse.Value.Errors
                .Select(e => e.ToError())
                .ToList();
        }

        return Result.Success;
    }

    #endregion

    #region HTTP Request Handling

    private async Task<ErrorOr<HttpResponseMessage>> SendRequestAsync(
        Func<HttpRequestMessage> requestFactory,
        CancellationToken cancellationToken)
    {
        try
        {
            var request = requestFactory();
            _logger.LogDebug("Sending {Method} request to {Url}", 
                request.Method, request.RequestUri);

            var response = await _httpClient.SendAsync(request, cancellationToken);

            _logger.LogDebug("Received {StatusCode} from {Url}",
                response.StatusCode, request.RequestUri);

            return response;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error occurred");
            return ApiErrors.Network.ConnectionFailed;
        }
        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
        {
            _logger.LogWarning("Request timed out");
            return ApiErrors.Network.Timeout;
        }
        catch (TaskCanceledException)
        {
            _logger.LogInformation("Request was cancelled");
            return ApiErrors.Network.Cancelled;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during HTTP request");
            return ApiErrors.Http.ServerError;
        }
    }

    #endregion

    #region Response Deserialization

    /// <summary>
    /// Deserializes ApiResponse{T} from the response body
    /// </summary>
    private async Task<ErrorOr<ApiResponse<TResponse>>> DeserializeApiResponseAsync<TResponse>(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        try
        {
            var apiResponse = await response.Content
                .ReadFromJsonAsync<ApiResponse<TResponse>>(_jsonOptions, cancellationToken);

            if (apiResponse is null)
            {
                _logger.LogWarning("Received null ApiResponse");
                return ApiErrors.Serialization.DeserializationFailed("ApiResponse");
            }

            return apiResponse;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize ApiResponse<{Type}>", 
                typeof(TResponse).Name);
            return ApiErrors.Serialization.DeserializationFailed(typeof(TResponse).Name);
        }
    }

    /// <summary>
    /// Deserializes ApiResponse (without data) from the response body
    /// </summary>
    private async Task<ErrorOr<ApiResponse>> DeserializeApiResponseAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        try
        {
            var apiResponse = await response.Content
                .ReadFromJsonAsync<ApiResponse>(_jsonOptions, cancellationToken);

            if (apiResponse is null)
            {
                _logger.LogWarning("Received null ApiResponse");
                return ApiErrors.Serialization.DeserializationFailed("ApiResponse");
            }

            return apiResponse;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize ApiResponse");
            return ApiErrors.Serialization.DeserializationFailed("ApiResponse");
        }
    }

    #endregion

    #region Response Unwrapping

    /// <summary>
    /// Unwraps ApiResponse{T} to ErrorOr{T}
    /// </summary>
    private static ErrorOr<TResponse> UnwrapApiResponse<TResponse>(ApiResponse<TResponse> apiResponse)
    {
        if (!apiResponse.IsSuccess)
        {
            return apiResponse.Errors
                .Select(e => e.ToError())
                .ToList();
        }

        if (apiResponse.Data is null)
        {
            return ApiErrors.Serialization.DeserializationFailed(typeof(TResponse).Name);
        }

        return apiResponse.Data;
    }

    #endregion

    #region Error Handling

    /// <summary>
    /// Checks if the status code indicates an error without ApiResponse body
    /// </summary>
    private static bool IsNonApiErrorStatus(HttpStatusCode statusCode)
    {
        return statusCode is 
            HttpStatusCode.BadGateway or 
            HttpStatusCode.ServiceUnavailable or 
            HttpStatusCode.GatewayTimeout;
    }

    /// <summary>
    /// Maps non-API error status codes to errors
    /// </summary>
    private static List<Error> HandleNonApiErrorStatus(HttpStatusCode statusCode)
    {
        return statusCode switch
        {
            HttpStatusCode.BadGateway => [ApiErrors.Http.ServiceUnavailable],
            HttpStatusCode.ServiceUnavailable => [ApiErrors.Http.ServiceUnavailable],
            HttpStatusCode.GatewayTimeout => [ApiErrors.Network.Timeout],
            _ => [ApiErrors.Http.ServerError]
        };
    }

    #endregion

    #region Request Building Helpers

    private HttpRequestMessage CreateJsonRequest<TRequest>(
        HttpMethod method, 
        string endpoint, 
        TRequest request)
    {
        var json = JsonSerializer.Serialize(request, _jsonOptions);
        var reu =  new HttpRequestMessage(method, endpoint)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
        return reu;
    }

    private static string BuildUrlWithQueryParams(
        string endpoint, 
        Dictionary<string, string>? parameters)
    {
        if (parameters is null || parameters.Count == 0)
        {
            return endpoint;
        }

        var queryString = string.Join("&",
            parameters
                .Where(kvp => !string.IsNullOrWhiteSpace(kvp.Value))
                .Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}")
        );

        return string.IsNullOrEmpty(queryString) 
            ? endpoint 
            : $"{endpoint}?{queryString}";
    }

    #endregion
}