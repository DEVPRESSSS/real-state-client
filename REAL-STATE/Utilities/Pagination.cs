using Microsoft.EntityFrameworkCore;
using REAL_STATE.Models.VM;

namespace REAL_STATE.Utilities;

public static class Pagination
{
    public static async Task<PagedResult<T>> ToPageAsync<T>(
        this IQueryable<T> query, int page = 1, int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        pageSize = Math.Clamp(pageSize, 1, 100);
        var total = await query.CountAsync(cancellationToken);
        page = Math.Clamp(page, 1, Math.Max(1, (int)Math.Ceiling(total / (double)pageSize)));
        return new PagedResult<T>
        {
            Items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken),
            CurrentPage = page, PageSize = pageSize, TotalItems = total
        };
    }
}
