using Adopetz.Api.Application.Common.Exceptions;
using Adopetz.Api.Domain.Common.Constants;
using Adopetz.Api.Domain.Common.Interfaces;
using Adopetz.Api.Domain.Interfaces;
using MediatR;

namespace Adopetz.Api.Application.Features.Pets.Commands
{
    public class DeletePetImageCommandHandler(
        IPetRepository petRepository,
        IFileStorageService fileStorageService,
        IUnitOfWork uow) : IRequestHandler<DeletePetImageCommand>
    {
        public async Task Handle(DeletePetImageCommand request, CancellationToken cancellationToken)
        {
            var pet = await petRepository.GetByIdAsync(request.PetId, cancellationToken);

            if (pet == null)
            {
                throw new NotFoundException($"Pet with ID {request.PetId} not found.", ErrorCodes.Pet.NotFound);
            }

            if (pet.UserId != request.UserId)
            {
                throw new ForbiddenAccessException("You do not have permission to modify this pet's images.", ErrorCodes.Common.ForbiddenAccess);
            }

            var filePath = pet.RemoveImage(request.ImageId);

            try
            {
                await fileStorageService.DeleteImageAsync(filePath, cancellationToken);
            }
            catch { }

            await uow.CommitAsync(cancellationToken);
        }
    }
}
