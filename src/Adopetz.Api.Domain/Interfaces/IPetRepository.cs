using Adopetz.Api.Domain.Common.Models;
using Adopetz.Api.Domain.Entities;

namespace Adopetz.Api.Domain.Interfaces
{
    public interface IPetRepository
    {
        Task<Pet?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<PaginatedList<Pet>> SearchPetsAsync(int pageNumber, int pageSize, string? searchTerm = null, CancellationToken cancellationToken = default);
        void Add(Pet pet);
        void Delete(Pet pet);
    }
}
