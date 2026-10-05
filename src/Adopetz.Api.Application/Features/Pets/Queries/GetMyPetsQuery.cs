using Adopetz.Api.Application.Common.Models;
using Adopetz.Api.Application.Features.Pets.Queries.Responses;
using Adopetz.Api.Domain.Common.Models;
using MediatR;

namespace Adopetz.Api.Application.Features.Pets.Queries
{
    public record GetMyPetsQuery(Guid UserId, PaginationQuery Pagination) : IRequest<PaginatedList<ManagerPetResponse>>;
}
