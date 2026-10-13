namespace Adopetz.Api.Application.Features.Pets.Queries.Responses
{
    public record PublicPetResponse(
        string Name,
        string About,
        bool IsAdopted,
        string? Breed,
        int? AgeInYears,
        DateTime? RescueDate,
        UserResponse Manager,
        IReadOnlyList<PublicVaccineResponse> Vaccines,
        IReadOnlyList<PublicPetImageResponse> Images
    );
}
