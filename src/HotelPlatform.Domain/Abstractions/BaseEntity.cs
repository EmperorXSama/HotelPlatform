using System.ComponentModel.DataAnnotations.Schema;

namespace HotelPlatform.Domain.Abstractions;
public abstract class BaseEntity<TId> : BaseEntity, IEquatable<BaseEntity<TId>>
    where TId : notnull
{
    public TId Id { get; protected set; }

    protected BaseEntity(TId id)
    {
        Id = id;
    }

    protected BaseEntity() 
    { 
        Id = default!;
    }

    public bool Equals(BaseEntity<TId>? other)
    {
        if (other is null) return false;
        if (other.GetType() != GetType()) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id.Equals(other.Id);
    }

    public override bool Equals(object? obj) => obj is BaseEntity<TId> other && Equals(other);
    
    public override int GetHashCode() => Id.GetHashCode();

    public static bool operator ==(BaseEntity<TId>? left, BaseEntity<TId>? right) => Equals(left, right);
    
    public static bool operator !=(BaseEntity<TId>? left, BaseEntity<TId>? right) => !Equals(left, right);
}
public class BaseEntity
{
    private readonly List<BaseDomainEvent> _domainEvents = new();
    [NotMapped]
    public IReadOnlyCollection<BaseDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
    
    public void AddDomainEvent(BaseDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
    public void RemoveDomainEvent(BaseDomainEvent domainEvent) => _domainEvents.Remove(domainEvent);
    public void ClearDomainEvents() => _domainEvents.Clear();
    
}