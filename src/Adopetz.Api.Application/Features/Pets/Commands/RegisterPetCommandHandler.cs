using Adopetz.Api.Application.Common.Exceptions;
using Adopetz.Api.Application.Common.Interfaces;
using Adopetz.Api.Domain.Common.Constants;
using Adopetz.Api.Domain.Common.Exceptions;
using Adopetz.Api.Domain.Common.Interfaces;
using Adopetz.Api.Domain.Entities;
using Adopetz.Api.Domain.Interfaces;
using MediatR;

namespace Adopetz.Api.Application.Features.Pets.Commands
{
    public class RegisterPetCommandHandler(
        IPetRepository petRepository,
        IUserRepository userRepository,
        IImageProcessingService imageProcessingService,
        IUnitOfWork uow) : IRequestHandler<RegisterPetCommand, Guid>
    {
        public async Task<Guid> Handle(RegisterPetCommand request, CancellationToken cancellationToken)
        {
            var userExists = await userRepository.ExistsAsync(request.UserId, cancellationToken);

            if (!userExists)
            {
                throw new NotFoundException($"User with ID {request.UserId} not found.", ErrorCodes.User.NotFound);
            }

            var pet = Pet.Create(request.Name, request.Breed, request.About, request.RescueDate, request.UserId, request.AgeInYears);

            if (request.Vaccines != null)
            {
                foreach (var vaccineCmd in request.Vaccines)
                {
                    pet.AddVaccine(vaccineCmd.Name, vaccineCmd.ApplicationDate);
                }
            }

            if (request.PetImages != null && request.PetImages.Any())
            {
                if (request.PetImages.Count > 4)
                {
                    throw new DomainException("The system allows uploading no more than 4 images per profile.", ErrorCodes.Pet.Image.MaxImagesLimit);
                }

                foreach (var imageCmd in request.PetImages)
                {
                    var folderPath = $"adopetz_pets/{pet.Id}";

                    var imageUrl = await imageProcessingService.ProcessAndUploadAsync(
                        imageCmd.Base64Data,
                        folderPath,
                        imageCmd.ContentType,
                        cancellationToken
                    );

                    pet.AddImage(imageUrl);
                }
            }

            petRepository.Add(pet);

            await uow.CommitAsync(cancellationToken);

            return pet.Id;
        }
    }
}
