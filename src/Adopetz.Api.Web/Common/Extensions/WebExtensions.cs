using Adopetz.Api.Domain.Common.Constants;
using Adopetz.Api.Infrastructure.Persistence.Initializers;
using Adopetz.Api.Web.Common.Constants;
using Adopetz.Api.Web.Common.Exceptions;
using Adopetz.Api.Web.Common.OpenApi;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;

namespace Adopetz.Api.Web.Common.Extensions
{
    public static class WebExtensions
    {
        private const string AngularCorsPolicy = "AngularClientPolicy";

        extension (IServiceCollection services)
        {
            public IServiceCollection AddWebServices(IConfiguration configuration)
            {
                services.AddProblemDetails();

                services.AddCors(options =>
                {
                    options.AddPolicy(AngularCorsPolicy, policy =>
                    {
                        policy.WithOrigins("http://localhost:4200")
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                    });
                });

                services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidIssuer = configuration.GetValue<string>("JwtSettings:Issuer"),
                            ValidateAudience = true,
                            ValidAudience = configuration.GetValue<string>("JwtSettings:Audience"),
                            ValidateLifetime = true,
                            IssuerSigningKey = new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(configuration.GetValue<string>("JwtSettings:Token")!)),
                            ValidateIssuerSigningKey = true,
                            ClockSkew = TimeSpan.FromMinutes(configuration.GetValue<int>("JwtSettings:ClockSkewInMinutes")!)
                        };
                    });

                services.AddAuthorization();

                services.AddScoped<AdopetzContextInitializer>();

                services.AddControllers()
                    .AddJsonOptions(options =>
                    {
                        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseUpper));
                    });

                // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
                services.AddOpenApi("v1", options =>
                {
                    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
                });

                services.AddRateLimiter(options =>
                {
                    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                    {
                        return RateLimitPartition.GetFixedWindowLimiter(
                            partitionKey: httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "anonymous",
                            factory: _ => new FixedWindowRateLimiterOptions
                            {
                                PermitLimit = 20,
                                Window = TimeSpan.FromSeconds(5),
                                QueueLimit = 5
                            }
                        );
                    });
                });

                return services;
            }

            public IServiceCollection AddWebRateLimiting()
            {
                services.AddRateLimiter(options =>
                {
                    options.AddPolicy(RateLimitingPolicies.LoginStrict, httpContext =>
                    {
                        var clientIp = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                        return RateLimitPartition.GetFixedWindowLimiter(clientIp, _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 5,
                            Window = TimeSpan.FromMinutes(1),
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                            QueueLimit = 0
                        });
                    });

                    options.OnRejected = async (_, _) =>
                    {
                        throw new TooManyRequestsException("Too many login tries. Wait a minute and try again.", ErrorCodes.Common.TooManyRequests);
                    };
                });

                return services;
            }
        }

        extension (WebApplication app)
        {
            public async Task<WebApplication> UseWebPipeline()
            {
                using (var scope = app.Services.CreateScope())
                {
                    var initializer = scope.ServiceProvider.GetRequiredService<AdopetzContextInitializer>();
                    await initializer.InitializeAsync();
                }

                if (app.Environment.IsDevelopment())
                {
                    app.UseCors(AngularCorsPolicy);

                    app.MapOpenApi();
                    app.MapScalarApiReference(options =>
                    {
                        options.WithTitle("Adopetz API")
                            .WithTheme(ScalarTheme.DeepSpace)
                            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
                    });

                    app.MapGet("", () => Results.Redirect("/scalar"));
                }
                else
                {
                    app.UseHttpsRedirection();
                }

                app.UseRateLimiter();

                app.UseAuthentication();
                app.UseAuthorization();

                app.MapControllers().RequireAuthorization();

                return app;
            }
        }
    }
}
