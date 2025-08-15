using Microsoft.EntityFrameworkCore;

namespace BackEnd.Utils;

public static class PaginationHelper
{
    public static async Task<PagedResult<T>> PaginateAsync<T>(
        IQueryable<T> query, int pageNumber = 1, int pageSize = 10) where T : class
    {
        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<T>
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalRecords = totalCount,
            Items = items
        };
    }
}