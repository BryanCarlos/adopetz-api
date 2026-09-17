
using Adopetz.Api.Application;
using Adopetz.Api.Infrastructure;
using Adopetz.Api.Infrastructure.Persistence.Initializers;
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

            services.AddProblemDetails();

            services.AddInfrastructure(configuration);
            services.AddApplication();

            services.AddScoped<AdopetzContextInitializer>();

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var initializer = scope.ServiceProvider.GetRequiredService<AdopetzContextInitializer>();
                await initializer.InitializeAsync();
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();

                app.MapGet("", () => Results.Redirect("/scalar"));
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
