namespace Adopetz.Api.Application.Features.Pets.Commands.DTOs
{
    public record CreateVaccineRequest(
        string Name,
        DateTime? ApplicationDate
    );
}
