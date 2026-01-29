namespace HotelManagement.BlazorServer.Services;


public interface IHotelApiClient
{
    // Generic methods
    Task<TResponse?> GetAsync<TResponse>(string endpoint, CancellationToken cancellationToken = default);
    Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest data, CancellationToken cancellationToken = default);
    Task<TResponse?> PutAsync<TRequest, TResponse>(string endpoint, TRequest data, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string endpoint, CancellationToken cancellationToken = default);
    
    // Health check
    Task<bool> IsHealthyAsync(CancellationToken cancellationToken = default);
}