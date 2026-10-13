using MediatR;

namespace Adopetz.Api.Application.Features.Pets.Commands
{
    public record DeletePetImageCommand(
        Guid PetId,
        Guid ImageId,
        Guid UserId
    ) : IRequest;
}
