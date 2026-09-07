using Adopetz.Api.Domain.Common.Models;
using Adopetz.Api.Domain.Entities;
using Adopetz.Api.Domain.Interfaces;
using Adopetz.Api.Infrastructure.Persistence.Utils;
using Microsoft.EntityFrameworkCore;

namespace Adopetz.Api.Infrastructure.Persistence.Repositories
{
    internal class PetRepository(AdopetzContext context) : IPetRepository
    {
        public void Add(Pet pet)
        {
            context.Pets.Add(pet);
        }

        public void Delete(Pet pet)
        {
            context.Pets.Remove(pet);
        }

        public async Task<Pet?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await context.Pets.FindAsync(id, cancellationToken);
        }

        public Task<PaginatedList<Pet>> SearchAsync(int pageNumber, int pageSize, string? searchTerm = null, CancellationToken cancellationToken = default)
        {
            var query = context.Pets
                .AsNoTracking()
                .Include(p => p.User)
                .Include(p => p.Vaccines)
                .Include(p => p.Images)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim();
                var pattern = $"%{term}%";

                query = query.Where(p => 
                EF.Functions.ILike(
                    EF.Functions.Unaccent(p.Name), 
                    EF.Functions.Unaccent(pattern)
                    )
                );
            }

            return query.ToPaginatedListAsync(pageNumber, pageSize, cancellationToken);
        }
    }
}
