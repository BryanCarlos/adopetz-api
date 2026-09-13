namespace Adopetz.Api.Application.Features.Pets.Queries.Responses
{
    public record PetResponse(
        Guid Id,
        string Name,
        string? Breed,
        int? AgeInYears,
        DateTime? RescueDate,
        UserResponse Manager,
        IReadOnlyList<VaccineResponse> Vaccines,
        IReadOnlyList<PetImageResponse> Images
    );
}
