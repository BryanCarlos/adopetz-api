using MediatR;

namespace Adopetz.Api.Application.Features.Pets.Commands
{
    public record DeletePetCommand(
        Guid Id,
        Guid UserId
    ) : IRequest;
}
