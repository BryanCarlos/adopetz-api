using Adopetz.Api.Application.Common.Interfaces;
using Adopetz.Api.Application.Features.Pets.Commands;
using Adopetz.Api.Application.Features.Pets.Commands.DTOs;
using Adopetz.Api.Domain.Common.Exceptions;
using Adopetz.Api.Domain.Common.Interfaces;
using Adopetz.Api.Domain.Common.Models;
using Adopetz.Api.Domain.Entities;
using Adopetz.Api.Domain.Interfaces;

internal static class RegistrationImageChecks
{
    public static async Task ParallelUploads()
    {
        var images = new PendingImages();
        var repository = new RegistrationPets();
        var uow = new RegistrationUnitOfWork();
        using var cancellation = new CancellationTokenSource();
        var registration = Handler(repository, images, uow).Handle(Request(4), cancellation.Token);

        Check(images.Pending.Count == 4, "All uploads must start before the first one completes");
        Check(images.Pending.Select(x => x.Data).SequenceEqual(new[] { "photo-0", "photo-1", "photo-2", "photo-3" }), "Each photo must be uploaded once");
        Check(images.Pending.All(x => x.ContentType == "image/jpeg" && x.Token == cancellation.Token), "Upload metadata and cancellation must be forwarded");
        Check(images.Pending.Select(x => x.Folder).Distinct().Count() == 1, "All photos must share the pet folder");
        Check(repository.Added is null && uow.Commits == 0, "Persistence must wait for all photos");

        foreach (var index in new[] { 3, 1, 2 }) images.Pending[index].Completion.SetResult($"https://images.test/{index}.jpg");
        Check(!registration.IsCompleted, "Registration must wait for the final upload");
        images.Pending[0].Completion.SetResult("https://images.test/0.jpg");
        var id = await registration;
        Check(repository.Added?.Id == id && uow.Commits == 1, "Pet must be saved exactly once");
        Check(repository.Added!.Images.Select(x => x.FilePath).SequenceEqual(Enumerable.Range(0, 4).Select(x => $"https://images.test/{x}.jpg")), "Photo order must follow the request, not completion order");
        Check(images.Pending[0].Folder == $"adopetz_pets/{id}", "Uploads must use the registered pet ID");
    }

    public static async Task FailedUpload()
    {
        var images = new PendingImages();
        var repository = new RegistrationPets();
        var uow = new RegistrationUnitOfWork();
        var registration = Handler(repository, images, uow).Handle(Request(4), default);
        Check(images.Pending.Count == 4, "All uploads must start");
        images.Pending[0].Completion.SetException(new InvalidOperationException("Upload failed"));
        images.Pending[1].Completion.SetResult("https://images.test/1.jpg");
        images.Pending[2].Completion.SetResult("https://images.test/2.jpg");
        Check(!registration.IsCompleted, "An upload failure must not leave other uploads running after the handler finishes");
        images.Pending[3].Completion.SetResult("https://images.test/3.jpg");
        await Expect<InvalidOperationException>(registration);
        Check(repository.Added is null && uow.Commits == 0, "Failed registration must not persist a partial pet");
    }

    public static async Task ImageLimit()
    {
        var images = new PendingImages();
        var repository = new RegistrationPets();
        var uow = new RegistrationUnitOfWork();
        await Expect<DomainException>(Handler(repository, images, uow).Handle(Request(5), default));
        Check(images.Pending.Count == 0 && repository.Added is null && uow.Commits == 0, "Five photos must be rejected before uploads or persistence");
    }

    public static async Task NoImages()
    {
        var images = new PendingImages();
        var repository = new RegistrationPets();
        var uow = new RegistrationUnitOfWork();
        var id = await Handler(repository, images, uow).Handle(Request(0), default);
        Check(images.Pending.Count == 0 && repository.Added?.Id == id && uow.Commits == 1, "Registration without photos must still save the pet");
    }

    public static async Task CancelledUploads()
    {
        var images = new PendingImages();
        var repository = new RegistrationPets();
        var uow = new RegistrationUnitOfWork();
        using var cancellation = new CancellationTokenSource();
        var registration = Handler(repository, images, uow).Handle(Request(4), cancellation.Token);
        Check(images.Pending.Count == 4 && images.Pending.All(x => x.Token == cancellation.Token), "All uploads must receive the request token");
        cancellation.Cancel();
        foreach (var image in images.Pending) image.Completion.SetCanceled(image.Token);
        await Expect<OperationCanceledException>(registration);
        Check(repository.Added is null && uow.Commits == 0, "Cancelled registration must not persist");
    }

    private static RegisterPetCommand Request(int count) => new(
        Guid.NewGuid(), "Nina", "SRD", "Amigável", null, 1,
        Enumerable.Range(0, count).Select(i => new CreatePetImageRequest("image/jpeg", $"photo-{i}")).ToList(), []);

    private static RegisterPetCommandHandler Handler(RegistrationPets pets, PendingImages images, RegistrationUnitOfWork uow) =>
        new(pets, new ExistingUser(), images, uow);

    private static void Check(bool condition, string message)
    {
        if (!condition) throw new Exception(message);
    }

    private static async Task Expect<T>(Task task) where T : Exception
    {
        try { await task; } catch (T) { return; }
        throw new Exception($"Expected {typeof(T).Name}");
    }

    private sealed record PendingUpload(string Data, string Folder, string ContentType, CancellationToken Token)
    {
        public TaskCompletionSource<string> Completion { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
    }

    private sealed class PendingImages : IImageProcessingService
    {
        public List<PendingUpload> Pending { get; } = [];
        public Task<string> ProcessAndUploadAsync(string base64Data, string folderPath, string contentType, CancellationToken cancellationToken)
        {
            var upload = new PendingUpload(base64Data, folderPath, contentType, cancellationToken);
            Pending.Add(upload);
            return upload.Completion.Task;
        }
    }

    private sealed class RegistrationPets : IPetRepository
    {
        public Pet? Added { get; private set; }
        public void Add(Pet pet)
        {
            Check(Added is null, "Pet must only be added once");
            Added = pet;
        }
        public void Delete(Pet pet) => throw new NotSupportedException();
        public Task<Pet?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<PaginatedList<Pet>> SearchAsync(int pageNumber, int pageSize, PetFilter filter, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<PaginatedList<Pet>> GetPetsByUserIdAsync(Guid userId, int pageNumber, int pageSize, CancellationToken cancellationToken) => throw new NotSupportedException();
    }

    private sealed class RegistrationUnitOfWork : IUnitOfWork
    {
        public int Commits { get; private set; }
        public Task<int> CommitAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Commits++;
            return Task.FromResult(1);
        }
    }

    private sealed class ExistingUser : IUserRepository
    {
        public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(true);
        public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<User?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public void Add(User user) => throw new NotSupportedException();
        public void Delete(User user) => throw new NotSupportedException();
    }
}
