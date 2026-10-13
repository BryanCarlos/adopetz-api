namespace Adopetz.Api.Application.Features.Pets.Queries.Responses
{
    public record PublicVaccineResponse(
        string Name,
        DateTime? ApplicationDate
    );
}
