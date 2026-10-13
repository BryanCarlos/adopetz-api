namespace Adopetz.Api.Application.Features.Pets.Queries.Responses
{
    public record ManagerPetResponse(
        Guid Id,
        string Name,
        string About,
        bool IsAdopted,
        string? Breed,
        int? AgeInYears,
        DateTime? RescueDate,
        IReadOnlyList<ManagerVaccineResponse> Vaccines,
        IReadOnlyList<ManagerPetImageResponse> Images
    );
}
