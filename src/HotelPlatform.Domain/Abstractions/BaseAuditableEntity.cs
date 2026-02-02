namespace HotelPlatform.Domain.Abstractions;

public class BaseAuditableEntity<TId> : BaseEntity<TId>
where TId : notnull
{
    public DateTimeOffset CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTimeOffset LastModified { get; set; }

    public string? LastModifiedBy { get; set; }
    
    public void SetUpdated(DateTime utcNow) => LastModified = utcNow; 
    public void SetUpdated() => LastModified = DateTime.UtcNow; 

    public BaseAuditableEntity() : base()
    {
        
    }
    public BaseAuditableEntity(TId id) : base(id)
    {
        
    }
}