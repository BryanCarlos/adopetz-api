namespace Adopetz.Api.Application.Common.Models
{
    public record PaginationQuery
    {
        private const int _maxPageSize = 50;
        private int _pageSize = 20;
        private int _pageNumber = 1;

        public int PageNumber
        {
            get => _pageNumber;
            set => _pageNumber = value < 1 ? 1 : value;
        }

        public int PageSize
        {
            get => _pageSize;
            set
            {
                if (value > _maxPageSize)
                {
                    _pageSize = _maxPageSize;
                }
                else if (value < 1)
                {
                    _pageSize = 20;
                }
                else
                {
                    _pageSize = value;
                }
            }
        }

        public PaginationQuery() { }

        public PaginationQuery(int pageNumber, int pageSize)
        {
            this.PageNumber = pageNumber;
            this.PageSize = pageSize;
        }
    }
}
