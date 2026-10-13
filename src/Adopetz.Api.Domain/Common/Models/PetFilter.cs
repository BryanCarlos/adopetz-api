using Adopetz.Api.Domain.Common.Enums;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Adopetz.Api.Domain.Common.Models
{
    public record PetFilter
    {
        public string? SearchTerm {get; init; }
        public int? MinAge {get; init; }
        public int? MaxAge {get; init; }
        public AdoptionStatusFilter AdoptionStatus { get; init; } = AdoptionStatusFilter.Available;
    }
}
