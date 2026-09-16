using Adopetz.Api.Application.Common.Exceptions;
using Adopetz.Api.Application.Common.Interfaces;
using Adopetz.Api.Domain.Common.Constants;
using Adopetz.Api.Domain.Common.Exceptions;
using Adopetz.Api.Domain.Common.Interfaces;

namespace Adopetz.Api.Application.Common.Services
{
    public class ImageProcessingService(IFileStorageService fileStorageService) : IImageProcessingService
    {
        private const long MaxImageSizeInBytes = 10 * 1024 * 1024;

        public async Task<string> ProcessAndUploadAsync(string base64Data, string folderPath, string contentType, CancellationToken cancellationToken)
        {
            long imageSizeInBytes = (base64Data.Length * 3) / 4;

            if (imageSizeInBytes > MaxImageSizeInBytes)
            {
                throw new DomainException($"Image exceeds the security limit of 10MB. Send a smaller image.", ErrorCodes.Pet.Image.ImageTooLarge);
            }

            try
            {
                return await fileStorageService.UploadImageAsync(
                    base64Data,
                    folderPath,
                    contentType,
                    cancellationToken
                );
            }
            catch
            {
                throw new ExternalServiceException("Failed to communicate with the image storage service. Try again.", ErrorCodes.Common.ExternalServiceProblem);
            }
        }
    }
}
