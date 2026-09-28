
using Adopetz.Api.Application.Common.Mappers;
using Adopetz.Api.Application.Features.Pets.Mappers;
using Adopetz.Api.Application.Features.Pets.Queries.Responses;
using Adopetz.Api.Domain.Common.Models;
using Adopetz.Api.Domain.Interfaces;
using MediatR;

namespace Adopetz.Api.Application.Features.Pets.Queries
{
    public class GetMyPetsQueryHandler(IPetRepository petRepository) : IRequestHandler<GetMyPetsQuery, PaginatedList<ManagerPetResponse>>
    {
        public async Task<PaginatedList<ManagerPetResponse>> Handle(GetMyPetsQuery request, CancellationToken cancellationToken)
        {
            var pets = await petRepository.GetPetsByUserIdAsync(
                request.UserId,
                request.Pagination.PageNumber,
                request.Pagination.PageSize,
                cancellationToken
            );

            return pets.ToPaginatedResponseList(p => p.ToManagerResponse());
        }
    }
}
