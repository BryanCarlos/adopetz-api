
using Adopetz.Api.Application;
using Adopetz.Api.Infrastructure;
using Adopetz.Api.Infrastructure.Persistence.Initializers;
using Adopetz.Api.Web.Common.Extensions;
using Scalar.AspNetCore;

namespace Adopetz.Api.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var configuration = builder.Configuration;
            var services = builder.Services;

            services.AddInfrastructure(configuration);
            services.AddApplication();
            services.AddWebServices(configuration);

            var app = builder.Build();

            await app.UseWebPipeline();

            app.Run();
        }
    }
}
