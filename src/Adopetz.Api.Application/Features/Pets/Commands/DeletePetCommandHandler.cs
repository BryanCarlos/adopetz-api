using Adopetz.Api.Application.Common.Exceptions;
using Adopetz.Api.Domain.Common.Constants;
using Adopetz.Api.Domain.Common.Interfaces;
using Adopetz.Api.Domain.Interfaces;
using MediatR;

namespace Adopetz.Api.Application.Features.Pets.Commands
{
    public class DeletePetCommandHandler(
        IPetRepository petRepository,
        IFileStorageService fileStorageService,
        IUnitOfWork uow) : IRequestHandler<DeletePetCommand>
    {
        public async Task Handle(DeletePetCommand request, CancellationToken cancellationToken)
        {
            var pet = await petRepository.GetByIdAsync(request.Id);

            if (pet == null)
            {
                throw new NotFoundException($"Pet with ID {request.Id} not found.", ErrorCodes.Pet.NotFound);
            }

            if (pet.UserId != request.UserId)
            {
                throw new ForbiddenAccessException("You do not have permission to delete this pet.", ErrorCodes.Common.ForbiddenAccess);
            }

            if (pet.Images.Any())
            {
                foreach (var image in pet.Images)
                {
                    try
                    {
                        await fileStorageService.DeleteImageAsync(image.FilePath, cancellationToken);
                    }
                    catch { }
                }
            }

            petRepository.Delete(pet);

            await uow.CommitAsync(cancellationToken);
        }
    }
}
