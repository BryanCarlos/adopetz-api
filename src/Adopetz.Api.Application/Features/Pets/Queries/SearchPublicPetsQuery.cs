using Adopetz.Api.Application.Common.Models;
using Adopetz.Api.Application.Features.Pets.Queries.Responses;
using Adopetz.Api.Domain.Common.Models;
using MediatR;

namespace Adopetz.Api.Application.Features.Pets.Queries
{
    public record SearchPublicPetsQuery : IRequest<PaginatedList<PublicPetResponse>>
    {
        public PaginationQuery Pagination { get; init; } = new PaginationQuery();
        public PetFilter Filter { get; init; } = new PetFilter();
    }
}
