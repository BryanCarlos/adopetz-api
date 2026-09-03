namespace Adopetz.Api.Domain.Common.Models
{
    public class PaginatedList<T>
    {
        public IReadOnlyCollection<T> Items { get; }
        public int PageNumber { get; }
        public int TotalPages { get; }
        public int TotalCount { get; }

        private PaginatedList(IReadOnlyCollection<T> items, int pageNumber, int totalPages, int totalCount)
        {
            this.Items = items;
            this.PageNumber = pageNumber;
            this.TotalPages = totalPages;
            this.TotalCount = totalCount;
        }

        public static PaginatedList<T> Create(IReadOnlyCollection<T> items, int pageNumber, int totalPages, int totalCount)
        {
            return new PaginatedList<T>(items, pageNumber, totalPages, totalCount);
        }
    }
}
