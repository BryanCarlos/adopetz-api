using Adopetz.Api.Application.Common.Exceptions;
using Adopetz.Api.Domain.Common.Constants;
using Adopetz.Api.Domain.Common.Interfaces;
using Adopetz.Api.Domain.Interfaces;
using MediatR;

namespace Adopetz.Api.Application.Features.Pets.Commands
{
    public class UpdatePetCommandHandler(
        IPetRepository petRepository,
        IUnitOfWork uow) : IRequestHandler<UpdatePetCommand>
    {
        public async Task Handle(UpdatePetCommand request, CancellationToken cancellationToken)
        {
            var pet = await petRepository.GetByIdAsync(request.Id, cancellationToken);

            if (pet == null)
            {
                throw new NotFoundException($"Pet with ID {request.Id} not found.", ErrorCodes.Pet.NotFound);
            }

            if (pet.UserId != request.UserId)
            {
                throw new ForbiddenAccessException("You do not have permission to update this pet.", ErrorCodes.Common.ForbiddenAccess);
            }

            pet.Update(request.Name, request.Adopted, request.Breed, request.RescueDate, request.AgeInYears);

            await uow.CommitAsync(cancellationToken);
        }
    }
}
