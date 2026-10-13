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
            var segments = uri.AbsolutePath.Split('/');

            var uploadIndex = Array.IndexOf(segments, "upload");
            if (uploadIndex == -1)
            {
                return;
            }

            var startIndex = uploadIndex + 1;
            if (segments[startIndex].StartsWith('v') && segments[startIndex].Length > 1 && char.IsDigit(segments[startIndex][1]))
            {
                startIndex++; // Ignora a versão
            }

            var publicIdSegments = segments.Skip(startIndex).ToArray();
            var publicIdWithExtension = string.Join("/", publicIdSegments);

            int lastDotIndex = publicIdWithExtension.LastIndexOf('.');
            var publicId = lastDotIndex != -1
                ? publicIdWithExtension.Substring(0, lastDotIndex)
                : publicIdWithExtension;

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
