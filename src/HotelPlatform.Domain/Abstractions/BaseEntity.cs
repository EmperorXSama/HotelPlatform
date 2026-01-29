using System.ComponentModel.DataAnnotations.Schema;

namespace HotelPlatform.Domain.Abstractions;

public class BaseEntity
{
    public Guid Id { get; set; }
    private readonly List<IBaseEvent> _domainEvents = new();
    [NotMapped]
    public IReadOnlyCollection<IBaseEvent> DomainEvents => _domainEvents.AsReadOnly();
    
    public void AddDomainEvent(IBaseEvent domainEvent) => _domainEvents.Add(domainEvent);
    public void RemoveDomainEvent(IBaseEvent domainEvent) => _domainEvents.Remove(domainEvent);
    public void ClearDomainEvents() => _domainEvents.Clear();

    protected BaseEntity(Guid? id)
    {
        Id = id ?? Guid.NewGuid();
    }
    
    
    protected BaseEntity(){}
    public bool Equals(BaseEntity? other)
    {
        if (other is null) return false;
        if (other.GetType() != GetType()) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id.Equals(other.Id);
    }

    public override bool Equals(object? obj)
    {
        return obj is BaseEntity other && Equals(other);
    }
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
    
    public static bool operator ==(BaseEntity? left, BaseEntity? right)
    {
        return Equals(left, right);
    }
    public static bool operator !=(BaseEntity? left, BaseEntity? right)
    {
        return !Equals(left, right);
    }

}