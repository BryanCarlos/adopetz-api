using Adopetz.Api.Application.Features.Pets.Queries.Responses;
using Adopetz.Api.Domain.Entities;

namespace Adopetz.Api.Application.Features.Pets.Mappers
{
    public static class PetExtensions
    {
        extension (Pet pet)
        {
            public PublicPetResponse ToResponse()
            {
                return new PublicPetResponse
                (
                    Name: pet.Name,
                    About: pet.About,
                    IsAdopted: pet.Adopted,
                    Breed: pet.Breed,
                    AgeInYears: pet.AgeInYears,
                    RescueDate: pet.RescueDate,
                    Manager: pet.User.ToResponse(),
                    Vaccines: pet.Vaccines.Select(v => v.ToResponse()).ToList(),
                    Images: pet.Images.Select(i => i.ToResponse()).ToList()
                );
            }

            public ManagerPetResponse ToManagerResponse()
            {
                return new ManagerPetResponse
                (
                    Id: pet.Id,
                    Name: pet.Name,
                    IsAdopted: pet.Adopted,
                    About: pet.About,
                    Breed: pet.Breed,
                    AgeInYears: pet.AgeInYears,
                    RescueDate: pet.RescueDate,
                    Vaccines: pet.Vaccines.Select(v => v.ToManagerResponse()).ToList(),
                    Images: pet.Images.Select(i => i.ToManagerResponse()).ToList()
                );
            }
        }
    }
}
