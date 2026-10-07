using System.Linq.Expressions;
using Adopetz.Api.Application.Common.Exceptions;
using Adopetz.Api.Application.Features.Pets.Commands;
using Adopetz.Api.Application.Features.Pets.Commands.Validators;
using Adopetz.Api.Application.Features.Pets.Queries;
using Adopetz.Api.Domain.Common.Interfaces;
using Adopetz.Api.Domain.Common.Models;
using Adopetz.Api.Domain.Entities;
using Adopetz.Api.Domain.Interfaces;
using Adopetz.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Adopetz.Api.Web.Infrastructure.Middlewares;
using Adopetz.Api.Domain.Common.Constants;
using Adopetz.Api.Application.Common.Interfaces;
using Adopetz.Api.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

// Standalone regression runner: no external test packages, database, credentials or uploads.
var organization = Guid.NewGuid();
var otherOrganization = Guid.NewGuid();
var pet = Pet.Create("Thor", "SRD", "Amigável", null, organization, 2);
var repository = new PetStub(pet);
var unitOfWork = new UnitOfWorkStub();
var tests = new (string Name, Func<Task> Run)[]
{
    ("Refresh token configuration generates random tokens with a future expiry", () =>
    {
        using var services = CreateTokenServices("32", "7");
        var generator = services.GetRequiredService<ITokenGenerator>();
        var now = DateTime.UtcNow;
        var first = generator.GenerateRefreshToken(now);
        var second = generator.GenerateRefreshToken(now);
        Assert(Convert.FromBase64String(first.Token).Length == 32, "Configured token length must be applied");
        Assert(first.Token != second.Token, "Refresh tokens must be random");
        Assert(first.ExpiresAt == now.AddDays(7), "Configured refresh lifetime must be applied");
        return Task.CompletedTask;
    }),
    ("Missing or zero refresh settings are rejected instead of generating empty cookies", async () =>
    {
        foreach (var settings in new (string? Bytes, string? Days)[] { (null, null), ("0", "7"), ("32", "0") })
        {
            using var services = CreateTokenServices(settings.Bytes, settings.Days);
            await Throws<OptionsValidationException>(() =>
            {
                services.GetRequiredService<ITokenGenerator>();
                return Task.CompletedTask;
            });
        }
    }),
    ("Problem details uses the actual HTTP status for expired sessions", async () =>
    {
        var writer = new ProblemWriter();
        var handler = new GlobalExceptionHandler(NullLogger<GlobalExceptionHandler>.Instance, writer);
        var context = new DefaultHttpContext();
        await handler.TryHandleAsync(context, new UnauthorizedException("Expired", ErrorCodes.Common.UnauthorizedAccess), default);
        Assert(context.Response.StatusCode == 401 && writer.Status == 401, "Expired refresh tokens must return HTTP 401, not HTTP 500");
    }),
    ("Pet age accepts null and 0..99, rejects negative and 100", () =>
    {
        foreach (int? age in new int?[] { null, 0, 2, 99, -1, 100 })
        {
            var register = new RegisterPetCommand(organization, "Nina", null, "Amigável", null, age, [], []);
            var update = new UpdatePetCommand(pet.Id, organization, "Nina", false, null, "Amigável", null, age);
            var expected = age is null or >= 0 and < 100;
            Assert(new RegisterPetCommandValidator().Validate(register).IsValid == expected, $"Registration age {age}");
            Assert(new UpdatePetCommandValidator().Validate(update).IsValid == expected, $"Update age {age}");
        }
        return Task.CompletedTask;
    }),
    ("Managed detail refuses another ONG and returns the owner's pet", async () =>
    {
        var handler = new GetManagedPetQueryHandler(repository);
        await Throws<NotFoundException>(() => handler.Handle(new GetManagedPetQuery(pet.Id, otherOrganization), default));
        var own = await handler.Handle(new GetManagedPetQuery(pet.Id, organization), default);
        Assert(own.Id == pet.Id && own.Name == "Thor", "Owner should receive their pet");
    }),
    ("Update refuses another ONG without changing the pet or committing", async () =>
    {
        var handler = new UpdatePetCommandHandler(repository, unitOfWork);
        await Throws<ForbiddenAccessException>(() => handler.Handle(new UpdatePetCommand(pet.Id, otherOrganization, "Unauthorized edit", true, null, "Changed", null, 2), default));
        Assert(pet.Name == "Thor" && !pet.Adopted && unitOfWork.Commits == 0, "Forbidden mutation must not persist");
    }),
    ("My pets applies the ownership predicate before pagination", async () =>
    {
        var capture = new CaptureQuery();
        var options = new DbContextOptionsBuilder<AdopetzContext>()
            .UseNpgsql("Host=127.0.0.1;Database=unused;Username=unused;Password=unused")
            .AddInterceptors(capture).Options;
        await using var context = new AdopetzContext(options);
        var type = typeof(AdopetzContext).Assembly.GetType("Adopetz.Api.Infrastructure.Persistence.Repositories.PetRepository")!;
        var realRepository = (IPetRepository)Activator.CreateInstance(type, context)!;
        try { await realRepository.GetPetsByUserIdAsync(organization, 1, 9, default); }
        catch (QueryCaptured) { }
        var finder = new OwnerPredicateFinder();
        finder.Visit(capture.Expression);
        Assert(finder.Found, "The pagination/count query must filter Pet.UserId");
    }),
    ("Vaccine update refuses future dates without changing the record", () =>
    {
        var vaccine = Vaccine.Create(pet, "Antirrábica", null);
        try { vaccine.Update("New name", DateTime.UtcNow.AddDays(1)); throw new Exception("Future vaccine date was accepted"); }
        catch (Adopetz.Api.Domain.Common.Exceptions.DomainException) { }
        Assert(vaccine.Name == "Antirrábica", "Invalid edit must not mutate vaccine");
        return Task.CompletedTask;
    }),
};
foreach (var test in tests) { await test.Run(); Console.WriteLine($"PASS: {test.Name}"); }
Console.WriteLine($"{tests.Length} regression checks passed.");

static void Assert(bool condition, string message) { if (!condition) throw new Exception(message); }
static ServiceProvider CreateTokenServices(string? bytes, string? days)
{
    var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
    {
        ["Database:Host"] = "localhost",
        ["Database:Port"] = "5432",
        ["Database:Name"] = "unused",
        ["Database:Username"] = "unused",
        ["Database:Password"] = "unused",
        ["CLOUDINARY_URL"] = "cloudinary://unused:unused@unused",
        ["RefreshTokenSettings:TokenBytes"] = bytes,
        ["RefreshTokenSettings:ExpiryDays"] = days,
    }).Build();
    var services = new ServiceCollection();
    services.AddInfrastructure(configuration);
    return services.BuildServiceProvider();
}
static async Task Throws<T>(Func<Task> action) where T : Exception
{
    try { await action(); } catch (T) { return; }
    throw new Exception($"Expected {typeof(T).Name}");
}
sealed class QueryCaptured : Exception;
sealed class CaptureQuery : IQueryExpressionInterceptor
{
    public Expression? Expression { get; private set; }
    public Expression QueryCompilationStarting(Expression queryExpression, QueryExpressionEventData eventData)
    {
        Expression = queryExpression;
        throw new QueryCaptured(); // Stop before any database connection is opened.
    }
}
sealed class OwnerPredicateFinder : ExpressionVisitor
{
    public bool Found { get; private set; }
    protected override Expression VisitMethodCall(MethodCallExpression node)
    {
        if (node.Method.Name == "Where" && node.Arguments.Count == 2 &&
            node.Arguments[1] is UnaryExpression { Operand: LambdaExpression lambda } &&
            lambda.Body is BinaryExpression { NodeType: ExpressionType.Equal, Left: MemberExpression member } &&
            member.Member.Name == nameof(Pet.UserId)) Found = true;
        return base.VisitMethodCall(node);
    }
}
sealed class PetStub(Pet pet) : IPetRepository
{
    public Task<Pet?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult<Pet?>(id == pet.Id ? pet : null);
    public void Add(Pet value) => throw new NotSupportedException();
    public void Delete(Pet value) => throw new NotSupportedException();
    public Task<PaginatedList<Pet>> SearchAsync(int pageNumber, int pageSize, PetFilter filter, CancellationToken cancellationToken = default) => throw new NotSupportedException();
    public Task<PaginatedList<Pet>> GetPetsByUserIdAsync(Guid userId, int pageNumber, int pageSize, CancellationToken cancellationToken) => throw new NotSupportedException();
}
sealed class UnitOfWorkStub : IUnitOfWork
{
    public int Commits { get; private set; }
    public Task<int> CommitAsync(CancellationToken cancellationToken = default) { Commits++; return Task.FromResult(1); }
}
sealed class ProblemWriter : IProblemDetailsService
{
    public int? Status { get; private set; }
    public ValueTask WriteAsync(ProblemDetailsContext context) { Status = context.ProblemDetails.Status; return ValueTask.CompletedTask; }
    public ValueTask<bool> TryWriteAsync(ProblemDetailsContext context) { Status = context.ProblemDetails.Status; return ValueTask.FromResult(true); }
}
