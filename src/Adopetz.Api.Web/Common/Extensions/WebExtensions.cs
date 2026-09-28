using Adopetz.Api.Domain.Common.Constants;
using Adopetz.Api.Infrastructure.Persistence.Initializers;
using Adopetz.Api.Web.Common.Constants;
using Adopetz.Api.Web.Common.Exceptions;
using Adopetz.Api.Web.Common.OpenApi;
using Adopetz.Api.Web.Infrastructure.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
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
                services.Configure<KestrelServerOptions>(options =>
                {
                    options.Limits.MaxRequestBodySize = 2 * 1024 * 1024;
                });

                services.AddProblemDetails();
                services.AddExceptionHandler<GlobalExceptionHandler>();

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

                services.Configure<ApiBehaviorOptions>(options =>
                {
                    options.SuppressModelStateInvalidFilter = true;
                });

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

                services.AddHsts(options =>
                {
                    options.MaxAge = TimeSpan.FromDays(365);
                    options.IncludeSubDomains = true;
                    options.Preload = true;
                });

                return services;
            }

            public IServiceCollection AddWebRateLimiting()
            {
                services.AddRateLimiter(options =>
                {
                    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

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

                    options.AddPolicy(RateLimitingPolicies.PublicQueryStrict, httpContext =>
                    {
                        var identifier = string.Empty;

                        if (httpContext.User.Identity?.IsAuthenticated == true)
                        {
                            identifier = httpContext.User.GetUserId().ToString();
                        }
                        else
                        {
                            identifier = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                        }

                        return RateLimitPartition.GetSlidingWindowLimiter(identifier, _ => new SlidingWindowRateLimiterOptions
                        {
                            PermitLimit = 60,
                            QueueLimit = 0,
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                            SegmentsPerWindow = 6,
                            Window = TimeSpan.FromMinutes(1)
                        });
                    });

                    options.AddPolicy(RateLimitingPolicies.PetRegisterStrict, httpContext =>
                    {
                        var userId = httpContext.User.GetUserId();

                        return RateLimitPartition.Get(userId, _ =>
                            RateLimiter.CreateChained(
                                new ConcurrencyLimiter(new ConcurrencyLimiterOptions
                                {
                                    PermitLimit = 5,
                                    QueueLimit = 5,
                                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                                }),
                                new FixedWindowRateLimiter(new FixedWindowRateLimiterOptions
                                {
                                    PermitLimit = 100,
                                    Window = TimeSpan.FromDays(1),
                                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                                    QueueLimit = 0
                                })
                            )
                        );
                    });

                    options.AddPolicy(RateLimitingPolicies.PetImageRegisterStrict, httpContext =>
                    {
                        return RateLimitPartition.Get(httpContext.User.GetUserId(), _ =>
                            RateLimiter.CreateChained(
                                new ConcurrencyLimiter(new ConcurrencyLimiterOptions
                                {
                                    PermitLimit = 5,
                                    QueueLimit = 5,
                                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                                }),

                                new FixedWindowRateLimiter(new FixedWindowRateLimiterOptions
                                {
                                    PermitLimit = 300,
                                    Window = TimeSpan.FromDays(1),
                                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                                    QueueLimit = 0
                                })
                            )
                        );
                    });

                    options.OnRejected = (_, _) =>
                    {
                        throw new TooManyRequestsException("Too many tries. Try again later.", ErrorCodes.Common.TooManyRequests);
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

                    app.UseDeveloperExceptionPage();
                }
                else
                {
                    app.UseHsts();
                }

                app.UseHttpsRedirection();

                app.UseAuthentication();
                app.UseAuthorization();
                app.UseRateLimiter();

                app.MapControllers().RequireAuthorization();

                return app;
            }
        }
    }
}
