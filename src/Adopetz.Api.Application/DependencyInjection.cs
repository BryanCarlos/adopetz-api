using Adopetz.Api.Application.Common.Behaviors;
using Adopetz.Api.Application.Common.Interfaces;
using Adopetz.Api.Application.Common.Services;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Adopetz.Api.Application
{
    public static class DependencyInjection
    {
        extension(IServiceCollection services)
        {
            public IServiceCollection AddApplication()
            {

                services.AddMediatR(cfg =>
                {
                    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());

                    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
                });

                services.AddScoped<IImageProcessingService, ImageProcessingService>();

                return services;
            }
        }
    }
}
