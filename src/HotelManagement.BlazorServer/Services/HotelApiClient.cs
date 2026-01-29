using System.Net;
using System.Text.Json;

namespace HotelManagement.BlazorServer.Services;


public class HotelApiClient : IHotelApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<HotelApiClient> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public HotelApiClient(
        HttpClient httpClient,
        ILogger<HotelApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public async Task<TResponse?> GetAsync<TResponse>(
        string endpoint,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("GET {Endpoint}", endpoint);
            
            var response = await _httpClient.GetAsync(endpoint, cancellationToken);
            
            return await HandleResponseAsync<TResponse>(response, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GET request failed: {Endpoint}", endpoint);
            throw new ApiException($"Failed to GET {endpoint}", ex);
        }
    }

    public async Task<TResponse?> PostAsync<TRequest, TResponse>(
        string endpoint,
        TRequest data,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("POST {Endpoint}", endpoint);
            
            var response = await _httpClient.PostAsJsonAsync(endpoint, data, _jsonOptions, cancellationToken);
            
            return await HandleResponseAsync<TResponse>(response, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "POST request failed: {Endpoint}", endpoint);
            throw new ApiException($"Failed to POST {endpoint}", ex);
        }
    }

    public async Task<TResponse?> PutAsync<TRequest, TResponse>(
        string endpoint,
        TRequest data,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("PUT {Endpoint}", endpoint);
            
            var response = await _httpClient.PutAsJsonAsync(endpoint, data, _jsonOptions, cancellationToken);
            
            return await HandleResponseAsync<TResponse>(response, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "PUT request failed: {Endpoint}", endpoint);
            throw new ApiException($"Failed to PUT {endpoint}", ex);
        }
    }

    public async Task<bool> DeleteAsync(
        string endpoint,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("DELETE {Endpoint}", endpoint);
            
            var response = await _httpClient.DeleteAsync(endpoint, cancellationToken);
            
            if (response.StatusCode == HttpStatusCode.NotFound)
                return false;

            response.EnsureSuccessStatusCode();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DELETE request failed: {Endpoint}", endpoint);
            throw new ApiException($"Failed to DELETE {endpoint}", ex);
        }
    }

    public async Task<bool> IsHealthyAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync("/health", cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    private async Task<TResponse?> HandleResponseAsync<TResponse>(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            throw new ApiUnauthorizedException("Authentication required or token expired");
        }

        if (response.StatusCode == HttpStatusCode.Forbidden)
        {
            throw new ApiForbiddenException("You don't have permission to perform this action");
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return default;
        }

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError("API error {StatusCode}: {Content}", response.StatusCode, errorContent);
            throw new ApiException($"API returned {response.StatusCode}: {errorContent}");
        }

        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            return default;
        }

        return await response.Content.ReadFromJsonAsync<TResponse>(_jsonOptions, cancellationToken);
    }
}