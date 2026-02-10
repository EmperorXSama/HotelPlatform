using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using HotelManagement.BlazorServer.Settings;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;

namespace HotelManagement.BlazorServer.Services;

public class TokenService : ITokenService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly WebKeycloakSettings _keycloakSettings;
    private readonly ILogger<TokenService> _logger;

    private static readonly SemaphoreSlim RefreshLock = new(1, 1);

    public TokenService(
        IHttpContextAccessor httpContextAccessor,
        IHttpClientFactory httpClientFactory,
        IOptions<WebKeycloakSettings> keycloakSettings,
        ILogger<TokenService> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _httpClientFactory = httpClientFactory;
        _keycloakSettings = keycloakSettings.Value;
        _logger = logger;
    }

    private HttpContext? HttpContext => _httpContextAccessor.HttpContext;

    public async Task<string?> GetAccessTokenAsync()
    {
        if (HttpContext is null)
        {
            _logger.LogWarning("HttpContext is null - cannot retrieve access token");
            return null;
        }

        try
        {
            return await HttpContext.GetTokenAsync("access_token");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve access token");
            return null;
        }
    }

    public async Task<string?> GetRefreshTokenAsync()
    {
        if (HttpContext is null)
            return null;

        try
        {
            return await HttpContext.GetTokenAsync("refresh_token");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve refresh token");
            return null;
        }
    }

    public async Task<bool> IsTokenExpiredAsync()
    {
        var token = await GetAccessTokenAsync();
        if (string.IsNullOrEmpty(token))
            return true;

        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            // Add 60 second buffer - refresh before actual expiry
            return jwtToken.ValidTo <= DateTime.UtcNow.AddSeconds(60);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check token expiration");
            return true;
        }
    }

    public async Task<TokenRefreshResult> RefreshTokenIfNeededAsync()
    {
        if (HttpContext is null)
            return new TokenRefreshResult(false, ErrorMessage: "No HTTP context available");

        // Check if refresh is needed
        if (!await IsTokenExpiredAsync())
        {
            var currentToken = await GetAccessTokenAsync();
            return new TokenRefreshResult(true, currentToken);
        }

        // Use lock to prevent multiple simultaneous refresh attempts
        await RefreshLock.WaitAsync();
        try
        {
            // Double-check after acquiring lock (another thread might have refreshed)
            if (!await IsTokenExpiredAsync())
            {
                var currentToken = await GetAccessTokenAsync();
                return new TokenRefreshResult(true, currentToken);
            }

            return await PerformTokenRefreshAsync();
        }
        finally
        {
            RefreshLock.Release();
        }
    }

    private async Task<TokenRefreshResult> PerformTokenRefreshAsync()
    {
        var refreshToken = await GetRefreshTokenAsync();
        if (string.IsNullOrEmpty(refreshToken))
        {
            _logger.LogWarning("No refresh token available");
            return new TokenRefreshResult(false, ErrorMessage: "No refresh token available");
        }

        try
        {
            var client = _httpClientFactory.CreateClient();
            var tokenEndpoint = $"{_keycloakSettings.Authority}/protocol/openid-connect/token";

            var requestData = new Dictionary<string, string>
            {
                ["grant_type"] = "refresh_token",
                ["client_id"] = _keycloakSettings.ClientId,
                ["client_secret"] = _keycloakSettings.ClientSecret,
                ["refresh_token"] = refreshToken
            };

            var response = await client.PostAsync(
                tokenEndpoint,
                new FormUrlEncodedContent(requestData));

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning(
                    "Token refresh failed: {StatusCode} - {Error}",
                    response.StatusCode,
                    errorContent);

                return new TokenRefreshResult(
                    false,
                    ErrorMessage: "Token refresh failed. Please login again.");
            }

            var content = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonSerializer.Deserialize<KeycloakTokenResponse>(content);

            if (tokenResponse is null)
            {
                return new TokenRefreshResult(false, ErrorMessage: "Invalid token response");
            }

            // Update the stored tokens
            await UpdateStoredTokensAsync(tokenResponse);

            _logger.LogInformation("Successfully refreshed access token");

            return new TokenRefreshResult(true, tokenResponse.AccessToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Token refresh failed with exception");
            return new TokenRefreshResult(false, ErrorMessage: ex.Message);
        }
    }

    private async Task UpdateStoredTokensAsync(KeycloakTokenResponse tokenResponse)
    {
        if (HttpContext is null)
            return;

        var authenticateResult = await HttpContext.AuthenticateAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);

        if (!authenticateResult.Succeeded || authenticateResult.Properties is null)
            return;

        var tokens = new List<AuthenticationToken>
        {
            new() { Name = "access_token", Value = tokenResponse.AccessToken },
            new() { Name = "refresh_token", Value = tokenResponse.RefreshToken ?? "" },
            new() { Name = "token_type", Value = tokenResponse.TokenType ?? "Bearer" }
        };

        if (tokenResponse.ExpiresIn > 0)
        {
            var expiresAt = DateTimeOffset.UtcNow.AddSeconds(tokenResponse.ExpiresIn);
            tokens.Add(new AuthenticationToken
            {
                Name = "expires_at",
                Value = expiresAt.ToString("o")
            });
        }

        authenticateResult.Properties.StoreTokens(tokens);

        // Re-sign in to update the cookie
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            authenticateResult.Principal!,
            authenticateResult.Properties);
    }

    private class KeycloakTokenResponse
    {
        [System.Text.Json.Serialization.JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = string.Empty;

        [System.Text.Json.Serialization.JsonPropertyName("refresh_token")]
        public string? RefreshToken { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("token_type")]
        public string? TokenType { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }
    }
}