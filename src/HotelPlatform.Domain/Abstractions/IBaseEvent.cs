using MediatR;

namespace HotelPlatform.Domain.Abstractions;

public interface IBaseEvent : INotification
{
    DateTime OccuredOn { get; init; }
}