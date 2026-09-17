using Adopetz.Api.Domain.Common.Enums;
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
            return await context.Pets
                .Include(p => p.User)
                .Include(p => p.Vaccines)
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<PaginatedList<Pet>> GetPetsByUserIdAsync(Guid userId, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            return await context.Pets
                .AsNoTracking()
                .Include(p => p.User)
                .Include(p => p.Vaccines)
                .Include(p => p.Images)
                .OrderByDescending(p => p.RescueDate)
                .ToPaginatedListAsync(pageNumber, pageSize, cancellationToken);
        }

        public Task<PaginatedList<Pet>> SearchAsync(int pageNumber, int pageSize, PetFilter filter, CancellationToken cancellationToken = default)
        {
            var query = context.Pets
                .AsNoTracking()
                .Include(p => p.User)
                .Include(p => p.Vaccines)
                .Include(p => p.Images)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var term = filter.SearchTerm.Trim();

                query = query.Where(p =>
                    EF.Functions.ToTsVector("portuguese", p.Name + " " + p.About + " " + (p.Breed ?? ""))
                    .Matches(EF.Functions.PlainToTsQuery("portuguese", term))
                );
            }

            if (filter.MinAge.HasValue)
            {
                query = query.Where(p => p.AgeInYears != null && p.AgeInYears >= filter.MinAge);
            }
            if (filter.MaxAge.HasValue)
            {
                query = query.Where(p => p.AgeInYears != null && p.AgeInYears <= filter.MaxAge);
            }

            query = filter.AdoptionStatus switch
            {
                AdoptionStatusFilter.Available => query.Where(p => !p.Adopted),
                AdoptionStatusFilter.Adopted => query.Where(p => p.Adopted),
                _ => query
            };

            return query.ToPaginatedListAsync(pageNumber, pageSize, cancellationToken);
        }
    }
}
