namespace Adopetz.Api.Web.Models.Requests
{
    public record UpdatePetRequest(
        string Name,
        bool Adopted,
        string? Breed,
        string About,
        DateTime? RescueDate,
        int? AgeInYears
    );
}
