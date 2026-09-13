namespace Adopetz.Api.Application.Common.Interfaces
{
    public interface IImageProcessingService
    {
        Task<string> ProcessAndUploadAsync(
            string base64Data,
            string fileName,
            string contentType,
            CancellationToken cancellationToken);
    }
}
