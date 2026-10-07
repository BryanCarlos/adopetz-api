using Adopetz.Api.Application.Features.Pets.Queries.Responses;
using MediatR;

namespace Adopetz.Api.Application.Features.Pets.Queries
{
    public record GetManagedPetQuery(Guid PetId, Guid UserId) : IRequest<ManagerPetResponse>;
}
