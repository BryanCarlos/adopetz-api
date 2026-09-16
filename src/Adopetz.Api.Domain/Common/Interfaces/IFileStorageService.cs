namespace Adopetz.Api.Domain.Common.Interfaces
{
    public interface IFileStorageService
    {
        Task<string> UploadImageAsync(string base64Data, string folderPath, string contentType, CancellationToken cancellationToken);
        Task DeleteImageAsync(string imageUrl, CancellationToken cancellationToken);
    }
}
