namespace Adopetz.Api.Application.Features.Pets.Queries.Responses
{
    public record ManagerVaccineResponse(
        Guid Id,
        string Name,
        DateTime? ApplicationDate
    );
}
