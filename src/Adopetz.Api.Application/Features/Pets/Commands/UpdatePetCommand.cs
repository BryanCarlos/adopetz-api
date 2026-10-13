using MediatR;

namespace Adopetz.Api.Application.Features.Pets.Commands
{
    public record UpdatePetCommand(
        Guid Id,
        Guid UserId,
        string Name,
        bool Adopted,
        string? Breed,
        string About,
        DateTime? RescueDate,
        int? AgeInYears
    ) : IRequest;
}
