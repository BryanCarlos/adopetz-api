using Adopetz.Api.Domain.Common.Enums;

namespace Adopetz.Api.Domain.Common.Models
{
    public record PetFilter(
        string? SearchTerm = null,
        int? MinAge = null,
        int? MaxAge = null,
        AdoptionStatusFilter AdoptionStatus = AdoptionStatusFilter.Available
    );
}
