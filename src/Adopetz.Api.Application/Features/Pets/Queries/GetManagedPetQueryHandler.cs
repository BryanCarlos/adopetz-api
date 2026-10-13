using Adopetz.Api.Application.Common.Exceptions;
using Adopetz.Api.Application.Features.Pets.Mappers;
using Adopetz.Api.Application.Features.Pets.Queries.Responses;
using Adopetz.Api.Domain.Common.Constants;
using Adopetz.Api.Domain.Interfaces;
using MediatR;

namespace Adopetz.Api.Application.Features.Pets.Queries
{
    public class GetManagedPetQueryHandler(IPetRepository repository)
        : IRequestHandler<GetManagedPetQuery, ManagerPetResponse>
    {
        public async Task<ManagerPetResponse> Handle(GetManagedPetQuery request, CancellationToken cancellationToken)
        {
            var pet = await repository.GetByIdAsync(request.PetId, cancellationToken);
            if (pet == null || pet.UserId != request.UserId)
            {
                throw new NotFoundException("Pet not found for this organization.", ErrorCodes.Pet.NotFound);
            }
            return pet.ToManagerResponse();
        }
    }
}
