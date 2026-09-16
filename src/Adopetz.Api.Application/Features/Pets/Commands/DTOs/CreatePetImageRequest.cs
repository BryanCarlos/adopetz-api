namespace Adopetz.Api.Application.Features.Pets.Commands.DTOs
{
    public record CreatePetImageRequest(
        string ContentType,
        string Base64Data
    );
}
