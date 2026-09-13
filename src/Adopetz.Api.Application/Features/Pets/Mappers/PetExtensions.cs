using Adopetz.Api.Application.Features.Pets.Queries.Responses;
using Adopetz.Api.Domain.Entities;

namespace Adopetz.Api.Application.Features.Pets.Mappers
{
    public static class PetExtensions
    {
        extension (Pet pet)
        {
            public PetResponse ToResponse()
            {
                return new PetResponse
                (
                    Id: pet.Id,
                    Name: pet.Name,
                    Breed: pet.Breed,
                    AgeInYears: pet.AgeInYears,
                    RescueDate: pet.RescueDate,
                    Manager: pet.User.ToResponse(),
                    Vaccines: pet.Vaccines.Select(v => v.ToResponse()).ToList(),
                    Images: pet.Images.Select(i => i.ToResponse()).ToList()
                );
            }
        }
    }
}
