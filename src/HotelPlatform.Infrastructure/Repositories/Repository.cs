using ErrorOr;
using HotelPlatform.Application.Common.Interfaces;
using HotelPlatform.Domain.Abstractions;
using HotelPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HotelPlatform.Infrastructure.Repositories;


public class Repository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly ApplicationDbContext DbContext;

    protected Repository(ApplicationDbContext dbContext)
    {
        DbContext = dbContext;
    }

    public async Task<ErrorOr<Success>> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await DbContext.AddAsync(entity, cancellationToken);
        return Result.Success;
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbContext.Set<T>().AnyAsync(entity => entity.Id == id, cancellationToken: cancellationToken);
    }

    public async Task<ErrorOr<T?>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await DbContext.Set<T>().FirstOrDefaultAsync(entity=> entity.Id == id, cancellationToken);
        return entity;
    }

    public async Task<ErrorOr<Success>> UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
     
        var exists = await ExistsAsync(entity.Id, cancellationToken);
        if (!exists)
            return Error.NotFound($"{typeof(T).Name}.Id", $"{typeof(T).Name} with id {entity.Id} not found.");
    
        DbContext.Set<T>().Update(entity);
        return Result.Success;
    }

    public async  Task<ErrorOr<Success>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    { 
        var entity = await GetByIdAsync(id, cancellationToken);
        if (entity.IsError) return entity.Errors;
        if (entity.Value is null) return Error.NotFound($"{typeof(T).Name}.Id", $"{typeof(T).Name} with id {id} not found.");
        
        DbContext.Remove(entity);
        return Result.Success;
    }
}