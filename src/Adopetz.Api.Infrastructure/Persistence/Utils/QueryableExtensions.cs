using Adopetz.Api.Domain.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace Adopetz.Api.Infrastructure.Persistence.Utils
{
    public static class QueryableExtensions
    {
        extension<T>(IQueryable<T> query)
        {
            public async Task<PaginatedList<T>> ToPaginatedListAsync(
                int pageNumber,
                int pageSize,
                CancellationToken cancellationToken = default)
            {
                var totalCount = await query.CountAsync(cancellationToken);
                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                var items = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync(cancellationToken);

                return PaginatedList<T>.Create(items, pageNumber, totalPages, totalCount);
            }
        }
    }
}
