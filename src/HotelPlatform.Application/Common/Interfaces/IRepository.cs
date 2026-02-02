using HotelPlatform.Domain.Abstractions;

namespace HotelPlatform.Application.Common.Interfaces;

public interface IRepository<T,TId>  where T : BaseEntity<TId> where TId : IStronglyTypedId<Guid>
{
    Task<ErrorOr<Success>> AddAsync(T entity, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(TId id, CancellationToken cancellationToken = default);
    Task<ErrorOr<T?>> GetByIdAsync(TId id, CancellationToken cancellationToken = default);
    Task<ErrorOr<Success>> UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task<ErrorOr<Success>> DeleteAsync(TId id, CancellationToken cancellationToken = default);
    
}