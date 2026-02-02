namespace HotelPlatform.Domain.Abstractions;


public abstract class AggregateRoot<TId> : BaseAuditableEntity<TId>
    where TId : notnull
{

    protected AggregateRoot(TId id) : base(id)
    {
      
    }

    protected AggregateRoot() : base() 
    {
    }

   
}