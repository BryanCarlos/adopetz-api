namespace Adopetz.Api.Application.Features.Pets.Commands.DTOs
{
    public record CreatePetImageRequest(
        string FileName,
        string ContentType,
        string Base64Data
    );
}
