using Adopetz.Api.Application.Features.Users.Auth;
using Microsoft.Extensions.DependencyInjection;

namespace Adopetz.Api.Application
{
    public static class DependencyInjection
    {
        extension(IServiceCollection services)
        {
            public IServiceCollection AddApplication()
            {
                services.AddScoped<IUserAuthService, UserAuthService>();

                return services;
            }
        }
    }
}
