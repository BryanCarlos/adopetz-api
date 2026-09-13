using Adopetz.Api.Application.Common.Exceptions;
using Adopetz.Api.Application.Features.Pets.Mappers;
using Adopetz.Api.Application.Features.Pets.Queries.Responses;
using Adopetz.Api.Domain.Common.Constants;
using Adopetz.Api.Domain.Interfaces;
using MediatR;

namespace Adopetz.Api.Application.Features.Pets.Queries
{
    public class GetPetByIdQueryHandler(
        IPetRepository petRepository) : IRequestHandler<GetPetByIdQuery, PetResponse>
    {
        public async Task<PetResponse> Handle(GetPetByIdQuery request, CancellationToken cancellationToken)
        {
            var pet = await petRepository.GetByIdAsync(request.Id, cancellationToken);

            if (pet == null)
            {
                throw new NotFoundException($"Pet with ID {request.Id} not found.", ErrorCodes.Pet.NotFound);
            }

            return pet.ToResponse();
        }
    }
}
