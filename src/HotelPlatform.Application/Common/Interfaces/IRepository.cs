using HotelPlatform.Domain.Abstractions;

namespace HotelPlatform.Application.Common.Interfaces;

public interface IRepository<T>  where T : BaseEntity
{
    Task<ErrorOr<Success>> AddAsync(T entity, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ErrorOr<T?>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ErrorOr<Success>> UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task<ErrorOr<Success>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    
}