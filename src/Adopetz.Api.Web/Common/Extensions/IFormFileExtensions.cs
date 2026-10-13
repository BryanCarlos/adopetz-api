using Adopetz.Api.Application.Features.Pets.Commands.DTOs;

namespace Adopetz.Api.Web.Common.Extensions
{
    public static class IFormFileExtensions
    {
        extension (IFormFile file)
        {
            public async Task<CreatePetImageRequest> ToApplicationImageAsync()
            {
                using var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream);
                var fileBytes = memoryStream.ToArray();

                var base64String = Convert.ToBase64String(fileBytes);

                return new CreatePetImageRequest(file.ContentType, base64String);
            }
        }
    }
}
