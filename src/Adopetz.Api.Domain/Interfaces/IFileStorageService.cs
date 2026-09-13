namespace Adopetz.Api.Domain.Interfaces
{
    public interface IFileStorageService
    {
        Task<string> UploadImageAsync(string base64Data, string fileName, string contentType, CancellationToken cancellationToken);
        Task DeleteImageAsync(string imageUrl, CancellationToken cancellationToken);
    }
}
