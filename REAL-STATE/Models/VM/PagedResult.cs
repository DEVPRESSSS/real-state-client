namespace REAL_STATE.Models.VM
{
    public interface IPageInfo
    {
        int CurrentPage { get; }
        int PageSize { get; }
        int TotalItems { get; }
        int TotalPages { get; }
        bool HasPreviousPage { get; }
        bool HasNextPage { get; }
    }

    public class PagedResult<T> : IPageInfo
    {
        public IEnumerable<T> Items { get; set; } = [];

        public int CurrentPage { get; set; }

        public int PageSize { get; set; }

        public int TotalItems { get; set; }

        public int TotalPages =>
            PageSize > 0 ? (int)Math.Ceiling((double)TotalItems / PageSize) : 0;

        public bool HasPreviousPage => CurrentPage > 1;

        public bool HasNextPage => CurrentPage < TotalPages;
    }
}
