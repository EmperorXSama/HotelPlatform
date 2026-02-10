using ErrorOr;
using HotelPlatform.Application.Common.Interfaces;
using HotelPlatform.Application.Common.Messaging;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HotelPlatform.Application.Common.Behaviors;


public class UnitOfWorkBehavior<TRequest,TResponse>:IPipelineBehavior<TRequest,TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : IErrorOr
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TRequest> _logger;
    public UnitOfWorkBehavior(IServiceProvider serviceProvider, ILogger<TRequest> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async  Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {

        if (request is not ICommand)
        {
            _logger.LogInformation("we executing non command ");
            return await next(cancellationToken);
        }
            

        _logger.LogInformation("we executing command");
        var unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>();
        try
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);
            
            var response = await next(cancellationToken);
            _logger.LogInformation("we saved command");
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