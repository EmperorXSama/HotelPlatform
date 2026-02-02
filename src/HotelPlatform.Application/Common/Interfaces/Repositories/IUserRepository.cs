namespace HotelPlatform.Application.Common.Interfaces.Repositories;
public interface IUserRepository
{
    Task<User?> GetByIdAsync(UserId id, CancellationToken cancellationToken = default);
    Task<User?> GetByIdentityIdAsync(string identityId, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(UserId id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByIdentityIdAsync(string identityId, CancellationToken cancellationToken = default);
    Task AddAsync(User user, CancellationToken cancellationToken = default);
    void Update(User user);
}