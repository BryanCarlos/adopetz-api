using Adopetz.Api.Application.Common.Exceptions;
using Adopetz.Api.Application.Common.Interfaces;
using Adopetz.Api.Domain.Common.Constants;
using Adopetz.Api.Domain.Common.Exceptions;
using Adopetz.Api.Domain.Common.Interfaces;
using Adopetz.Api.Domain.Interfaces;
using MediatR;

namespace Adopetz.Api.Application.Features.Pets.Commands
{
    public class AddPetImageCommandHandler(
        IPetRepository petRepository,
        IImageProcessingService imageProcessingService,
        IUnitOfWork uow) : IRequestHandler<AddPetImageCommand>
    {
        public async Task Handle(AddPetImageCommand request, CancellationToken cancellationToken)
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

            if (pet.Images.Count >= 4)
            {
                throw new DomainException("The system allows uploading no more than 4 images per profile.", ErrorCodes.Pet.Image.MaxImagesLimit);
            }

            var imageUrl = await imageProcessingService.ProcessAndUploadAsync(
                request.Base64Data,
                request.FileName,
                request.ContentType,
                cancellationToken
            );

            pet.AddImage(imageUrl);

            await uow.CommitAsync(cancellationToken);
        }
    }
}
