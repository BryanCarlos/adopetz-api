using MediatR;

namespace Adopetz.Api.Application.Features.Pets.Commands
{
    public record AddVaccineCommand(
        Guid PetId,
        Guid UserId,
        string Name,
        DateTime? ApplicationDate
    ) : IRequest;
}
