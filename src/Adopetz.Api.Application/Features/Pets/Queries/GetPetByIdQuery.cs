using Adopetz.Api.Application.Features.Pets.Queries.Responses;
using MediatR;

namespace Adopetz.Api.Application.Features.Pets.Queries
{
    public record GetPetByIdQuery(Guid Id) : IRequest<PetResponse>;
}
