using Adopetz.Api.Application.Common.Interfaces;
using Adopetz.Api.Application.Common.Services;
using Adopetz.Api.Application.Features.Users.Auth;
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
                });

                services.AddScoped<IImageProcessingService, ImageProcessingService>();
                services.AddScoped<IUserAuthService, UserAuthService>();

                return services;
            }
        }
    }
}
