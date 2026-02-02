using MediatR;

namespace HotelPlatform.Domain.Abstractions;

public abstract record BaseDomainEvent : INotification
{
    public DateTime OccurredOn { get; init; }

    protected BaseDomainEvent()
    {
        OccurredOn = DateTime.UtcNow;
    }
}