using Adopetz.Api.Domain.Common.Models;

namespace Adopetz.Api.Application.Common.Mappers
{
    public static class PaginationListExtensions
    {
        extension<TEntity>(PaginatedList<TEntity> paginatedList)
        {
            public PaginatedList<TResponse> ToPaginatedResponseList<TResponse>(Func<TEntity, TResponse> mapFunction)
            {
                var responseItems = paginatedList.Items.Select(mapFunction).ToList();

                return PaginatedList<TResponse>.Create(
                    responseItems,
                    paginatedList.PageNumber,
                    paginatedList.TotalPages,
                    paginatedList.TotalCount
                );
            }
        }
    }
}
