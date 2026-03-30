using System;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Text;

using Microsoft.EntityFrameworkCore;

namespace Application.DTOs.Common;
public class PaginationResult<T> : ICollectionResponse<T>
{
    public List<T> Items { get; init; } = new List<T>();
    public int PageSize { get; init; }
    public int Page { get; init; }
    public int TotalCount { get; init; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasNextPage => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;
}
public static class Pagination
{
    public static async Task<PaginationResult<T>> CreateAsync<T>(
        IQueryable<T> query,
        int page,
        int pageSize)
    {
        int totalCount = await query.CountAsync();

        List<T> items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginationResult<T>
        {
            Items = items,
            PageSize = pageSize,
            Page = page,
            TotalCount = totalCount
        };
    }
}