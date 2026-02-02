// Infrastructure/Repositories/UserRepository.cs
using HotelPlatform.Application.Common.Interfaces.Repositories;
using HotelPlatform.Domain.Common.StronglyTypedIds;
using HotelPlatform.Domain.Users;
using HotelPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HotelPlatform.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _dbContext;

    public UserRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User?> GetByIdAsync(UserId id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<User?> GetByIdentityIdAsync(string identityId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .FirstOrDefaultAsync(u => u.IdentityId == identityId, cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == email.ToLowerInvariant(), cancellationToken);
    }

    public async Task<bool> ExistsAsync(UserId id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .AnyAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsByIdentityIdAsync(string identityId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .AnyAsync(u => u.IdentityId == identityId, cancellationToken);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await _dbContext.Users.AddAsync(user, cancellationToken);
    }

    public void Update(User user)
    {
        _dbContext.Users.Update(user);
    }
}