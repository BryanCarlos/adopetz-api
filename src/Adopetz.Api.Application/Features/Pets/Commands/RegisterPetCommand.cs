using Adopetz.Api.Application.Features.Pets.Commands.DTOs;
using MediatR;

namespace Adopetz.Api.Application.Features.Pets.Commands
{
    public record RegisterPetCommand(
        Guid UserId,
        string Name,
        string? Breed,
        string About,
        DateTime? RescueDate,
        int? AgeInYears,
        IList<CreatePetImageRequest> PetImages,
        IList<CreateVaccineRequest>? Vaccines
    ) : IRequest<Guid>;
}
