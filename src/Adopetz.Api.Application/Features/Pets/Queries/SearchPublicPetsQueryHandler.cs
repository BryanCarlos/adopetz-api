using Adopetz.Api.Application.Common.Mappers;
using Adopetz.Api.Application.Features.Pets.Mappers;
using Adopetz.Api.Application.Features.Pets.Queries.Responses;
using Adopetz.Api.Domain.Common.Models;
using Adopetz.Api.Domain.Interfaces;
using MediatR;

namespace Adopetz.Api.Application.Features.Pets.Queries
{
    public class SearchPublicPetsQueryHandler(IPetRepository petRepository) : IRequestHandler<SearchPublicPetsQuery, PaginatedList<PublicPetResponse>>
    {
        public async Task<PaginatedList<PublicPetResponse>> Handle(SearchPublicPetsQuery request, CancellationToken cancellationToken)
        {
            var pets = await petRepository.SearchAsync(
                pageNumber: request.Pagination.PageNumber,
                pageSize: request.Pagination.PageSize,
                filter: request.Filter,
                cancellationToken
            );

            return pets.ToPaginatedResponseList(p => p.ToResponse());
        }
    }
}
