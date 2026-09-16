using Adopetz.Api.Application.Common.Exceptions;
using Adopetz.Api.Domain.Common.Constants;
using Adopetz.Api.Domain.Common.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace Adopetz.Api.Infrastructure.Services
{
    public class CloudinaryStorageService(
        Cloudinary cloudinary) : IFileStorageService
    {
        public async Task DeleteImageAsync(string imageUrl, CancellationToken cancellationToken)
        {
            var uri = new Uri(imageUrl);
            var pathSegments = uri.AbsolutePath.Split('/');
            var fileNameWithExtension = pathSegments[^1];
            var publicId = Path.GetFileNameWithoutExtension(fileNameWithExtension);

            var deletionParams = new DeletionParams(publicId);

            await cloudinary.DestroyAsync(deletionParams);
        }

        public async Task<string> UploadImageAsync(
            string base64Data, 
            string folderPath, 
            string contentType, 
            CancellationToken cancellationToken)
        {
            var bytes = Convert.FromBase64String(base64Data);
            using var stream = new MemoryStream(bytes);

            var imageId = Guid.CreateVersion7().ToString();

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(imageId, stream),
                Folder = folderPath,
                PublicId = imageId,
                Overwrite = true,
                Transformation = new Transformation()
                    .Width(600)
                    .Height(600)
                    .Crop("fill")
                    .Gravity("auto")
                    .FetchFormat("auto")
                    .Quality("auto")
            };

            var uploadResult = await cloudinary.UploadAsync(uploadParams, cancellationToken);

            if (uploadResult.Error != null)
            {
                throw new ExternalServiceException("Error uploading image.", ErrorCodes.Pet.Image.FailedUpload);
            }

            return uploadResult.SecureUrl.ToString();
        }
    }
}
