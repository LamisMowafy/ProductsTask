using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Common
{
    public class PaginatedList<T> : List<T>
    {
        public int TotalCount { get; }
        public int TotalPages { get; }
        public int PageNumber { get; }
        public int PageSize { get; }

        public PaginatedList(List<T> items, int count, int pageNumber, int pageSize)
        {
            TotalCount = count;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            PageNumber = pageNumber;
            PageSize = pageSize;
            AddRange(items);
        }

        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
        {
            int count = await source.CountAsync();
            List<T> items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PaginatedList<T>(items, count, pageNumber, pageSize);
        }
    }

}
