using Adopetz.Api.Application.Features.Pets.Queries.Responses;
using Adopetz.Api.Domain.Entities;

namespace Adopetz.Api.Application.Features.Pets.Mappers
{
    public static class PetImageExtensions
    {
        extension (PetImage image)
        {
            public PetImageResponse ToResponse()
            {
                return new PetImageResponse(
                    Id: image.Id,
                    FilePath: image.FilePath
                );
            }
        }
    }
}
