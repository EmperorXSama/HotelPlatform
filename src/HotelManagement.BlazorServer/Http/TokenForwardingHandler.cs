using System.Net;
using System.Net.Http.Headers;
using HotelManagement.BlazorServer.Services;

namespace HotelManagement.BlazorServer.Http;

public class TokenForwardingHandler : DelegatingHandler
{
    private readonly ITokenService _tokenService;
    private readonly ILogger<TokenForwardingHandler> _logger;

    public TokenForwardingHandler(
        ITokenService tokenService,
        ILogger<TokenForwardingHandler> logger)
    {
        _tokenService = tokenService;
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        // Skip if Authorization header already exists
        if (request.Headers.Authorization is not null)
        {
            return await base.SendAsync(request, cancellationToken);
        }

        // Try to get a valid token (refreshing if needed)
        var refreshResult = await _tokenService.RefreshTokenIfNeededAsync();

        if (refreshResult.Success && !string.IsNullOrEmpty(refreshResult.AccessToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                refreshResult.AccessToken);

            _logger.LogDebug("Added Bearer token to request: {Url}", request.RequestUri);
        }
        else
        {
            _logger.LogWarning(
                "No valid access token for request: {Url}. Error: {Error}",
                request.RequestUri,
                refreshResult.ErrorMessage);
        }

        var response = await base.SendAsync(request, cancellationToken);

        // If we still get 401, the refresh token might be expired too
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            _logger.LogWarning(
                "API returned 401 even after token refresh for {Method} {Url}",
                request.Method,
                request.RequestUri);
        }

        return response;
    }
}