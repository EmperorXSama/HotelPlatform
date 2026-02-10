using System.Reflection;
using FluentValidation;
using HotelPlatform.Application.Common.Behaviors;
using HotelPlatform.Application.Common.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HotelPlatform.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services , IConfiguration configuration) =>
        services
            .AddMediatrServices();

    public static IServiceCollection AddMediatrServices(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();
        services.AddValidatorsFromAssembly(assembly);
        services.AddMediatR(mrc =>
        {
            mrc.RegisterServicesFromAssembly(assembly);
            mrc.AddOpenRequestPreProcessor(typeof(LoggingBehaviour<>));
            mrc.AddBehavior(typeof(IPipelineBehavior<,>),typeof(UnhandledExceptionBehaviour<,>));
            mrc.AddBehavior(typeof(IPipelineBehavior<,>),typeof(UnitOfWorkBehavior<,>));
            mrc.AddBehavior(typeof(IPipelineBehavior<,>),typeof(ValidationBehavior<,>));
            mrc.AddBehavior(typeof(IPipelineBehavior<,>),typeof(PerformanceBehaviour<,>));
        });
        return services;
    }
    

}