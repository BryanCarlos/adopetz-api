using MediatR;

namespace Adopetz.Api.Application.Features.Pets.Commands
{
    public record AddPetImageCommand(
        Guid PetId,
        Guid UserId,
        string Base64Data,
        string FileName,
        string ContentType
    ) : IRequest;
}
