using Adopetz.Api.Application.Features.Pets.Queries.Responses;
using Adopetz.Api.Domain.Entities;

namespace Adopetz.Api.Application.Features.Pets.Mappers
{
    public static class PetImageExtensions
    {
        extension (PetImage image)
        {
            public PublicPetImageResponse ToResponse()
            {
                return new PublicPetImageResponse(
                    FilePath: image.FilePath
                );
            }

            public ManagerPetImageResponse ToManagerResponse()
            {
                return new ManagerPetImageResponse(
                    Id: image.Id,
                    FilePath: image.FilePath
                );
            }
        }
    }
}
