namespace Adopetz.Api.Application.Features.Pets.Queries.Responses
{
    public record VaccineResponse(
        Guid Id,
        string Name,
        DateTime? ApplicationDate
    );
}
