using MediatR;

namespace Adopetz.Api.Application.Features.Pets.Commands
{
    public record UpdateVaccineCommand(
        Guid PetId,
        Guid VaccineId,
        Guid UserId,
        string Name,
        DateTime? ApplicationDate
    ) : IRequest;
}
