using MediatR;

namespace Adopetz.Api.Application.Features.Pets.Commands
{
    public record DeleteVaccineCommand(
        Guid PetId,
        Guid VaccineId,
        Guid UserId
    ) : IRequest;
}
