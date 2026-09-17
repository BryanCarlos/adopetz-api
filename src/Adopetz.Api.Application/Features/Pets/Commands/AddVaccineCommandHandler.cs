using Adopetz.Api.Application.Common.Exceptions;
using Adopetz.Api.Domain.Common.Constants;
using Adopetz.Api.Domain.Common.Interfaces;
using Adopetz.Api.Domain.Interfaces;
using MediatR;

namespace Adopetz.Api.Application.Features.Pets.Commands
{
    public class AddVaccineCommandHandler(
        IPetRepository petRepository,
        IUnitOfWork uow) : IRequestHandler<AddVaccineCommand>
    {
        public async Task Handle(AddVaccineCommand request, CancellationToken cancellationToken)
        {
            var pet = await petRepository.GetByIdAsync(request.PetId, cancellationToken);

            if (pet == null)
            {
                throw new NotFoundException($"Pet with ID {request.PetId} not found.", ErrorCodes.Pet.NotFound);
            }

            if (pet.UserId != request.UserId)
            {
                throw new ForbiddenAccessException("You do not have permission to modify this pet's vaccines.", ErrorCodes.Common.ForbiddenAccess);
            }

            pet.AddVaccine(request.Name, request.ApplicationDate);

            await uow.CommitAsync(cancellationToken);
        }
    }
}
