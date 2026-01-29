using ErrorOr;
using HotelPlatform.Application.Common.Interfaces;
using HotelPlatform.Application.Common.Messaging;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace HotelPlatform.Application.Common.Behaviors;


public class UnitOfWorkBehavior<TRequest,TResponse>:IPipelineBehavior<TRequest,TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : IErrorOr
{
    private readonly IServiceProvider _serviceProvider;

    public UnitOfWorkBehavior(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async  Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request is not ICommand)
            await next(cancellationToken);

        var unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>();
        try
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);
            
            var response = await next(cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            await unitOfWork.CommitTransactionAsync(cancellationToken);
            return response;
        }
        catch (Exception e)
        {
            await unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}