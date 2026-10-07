using Adopetz.Api.Application.Common.Interfaces;
using Adopetz.Api.Domain.Common.Interfaces;
using Adopetz.Api.Domain.Interfaces;
using Adopetz.Api.Infrastructure.Authentication;
using Adopetz.Api.Infrastructure.Authentication.Models;
using Adopetz.Api.Infrastructure.Persistence;
using Adopetz.Api.Infrastructure.Persistence.Repositories;
using Adopetz.Api.Infrastructure.Persistence.Utils;
using Adopetz.Api.Infrastructure.Services;
using CloudinaryDotNet;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Adopetz.Api.Infrastructure
{
    public static class DependencyInjection
    {
        extension(IServiceCollection services)
        {
            public IServiceCollection AddInfrastructure(IConfiguration configuration)
            {
                var connectionString = GetConnectionString(configuration);

                services.AddDbContext<AdopetzContext>(opt =>
                {
                    opt.UseNpgsql(connectionString);
                });

                services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
                services.AddOptions<RefreshTokenSettings>()
                    .Bind(configuration.GetSection(RefreshTokenSettings.SectionName))
                    .Validate(settings => settings.TokenBytes >= 32,
                        "RefreshTokenSettings:TokenBytes must be at least 32.")
                    .Validate(settings => settings.ExpiryDays > 0,
                        "RefreshTokenSettings:ExpiryDays must be greater than zero.")
                    .ValidateOnStart();

                services.AddSingleton<IPasswordHasher, PasswordHasher>();
                services.AddSingleton<ITokenGenerator, JwtTokenGenerator>();
                services.AddSingleton<IHasher, Hasher>();

                services.AddScoped<IUnitOfWork, UnitOfWork>();
                services.AddScoped<IUserRepository, UserRepository>();
                services.AddScoped<IPetRepository, PetRepository>();

                var cloudinaryUrl = configuration["CLOUDINARY_URL"];
                var cloudinary = new Cloudinary(cloudinaryUrl);
                services.AddSingleton(cloudinary);

                services.AddScoped<IFileStorageService, CloudinaryStorageService>();

                return services;
            }
        }

        private static string GetConnectionString(IConfiguration configuration)
        {
            var host = configuration["Database:Host"];
            var port = configuration["Database:Port"];
            var db = configuration["Database:Name"];
            var user = configuration["Database:Username"];
            var pass = configuration["Database:Password"];

            if (string.IsNullOrWhiteSpace(host) ||
                string.IsNullOrWhiteSpace(port) ||
                string.IsNullOrWhiteSpace(db) ||
                string.IsNullOrWhiteSpace(user) ||
                string.IsNullOrWhiteSpace(pass))
            {
                throw new NullReferenceException("Database connection data missing.");
            }

            return $"Host={host};Port={port};Database={db};Username={user};Password={pass}";
        }
    }
}
