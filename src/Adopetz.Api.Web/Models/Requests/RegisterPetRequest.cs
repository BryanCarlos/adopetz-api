using Adopetz.Api.Application.Features.Pets.Commands.DTOs;

namespace Adopetz.Api.Web.Models.Requests
{
    public record RegisterPetRequest(
        string Name,
        string? Breed,
        string About,
        DateTime? RescueDate,
        int? AgeInYears,
        IList<IFormFile> PetImages,
        IList<CreateVaccineRequest>? Vaccines
    );
}
