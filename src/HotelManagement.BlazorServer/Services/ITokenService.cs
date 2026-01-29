namespace HotelManagement.BlazorServer.Services;

public interface ITokenService
{
    Task<string?> GetAccessTokenAsync();
    Task<string?> GetRefreshTokenAsync();
    Task<bool> IsTokenExpiredAsync();
    Task<TokenRefreshResult> RefreshTokenIfNeededAsync();
}

public record TokenRefreshResult(
    bool Success,
    string? AccessToken = null,
    string? ErrorMessage = null);