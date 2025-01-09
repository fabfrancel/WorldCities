using Microsoft.EntityFrameworkCore;
using System.Reflection;
using WorldCities.Server.Extensions;

namespace WorldCities.Server.Data;

/// <summary>
/// Represents a paginated result of an API request.
/// </summary>
/// <typeparam name="T">The type of the data in the result.</typeparam>
public class ApiResult<T>
{
    /// <summary>
    /// Gets the data for the current page.
    /// </summary>
    public List<T> Data { get; private set; }

    /// <summary>
    /// Gets the index of the current page.
    /// </summary>
    public int PageIndex { get; private set; }

    /// <summary>
    /// Gets the size of the page.
    /// </summary>
    public int PageSize { get; private set; }

    /// <summary>
    /// Gets the total count of items.
    /// </summary>
    public int TotalCount { get; private set; }

    /// <summary>
    /// Gets the total number of pages.
    /// </summary>
    public int TotalPages { get; private set; }

    /// <summary>
    /// Gets a value indicating whether there is a previous page.
    /// </summary>
    public bool HasPreviousPage => PageIndex > 0;

    /// <summary>
    /// Gets a value indicating whether there is a next page.
    /// </summary>
    public bool HasNextPage => PageIndex + 1 < TotalPages;

    /// <summary>
    /// Gets or sets the column to sort by.
    /// </summary>
    public string? SortColumn { get; set; }

    /// <summary>
    /// Gets or sets the sort order.
    /// </summary>
    public string? SortOrder { get; set; }

    /// <summary>
    /// Gets or sets the column to filter by.
    /// </summary>
    public string? FilterColumn { get; set; }

    /// <summary>
    /// Gets or sets the filter query.
    /// </summary>
    public string? FilterQuery { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiResult{T}"/> class.
    /// </summary>
    /// <param name="data">The data for the current page.</param>
    /// <param name="count">The total count of items.</param>
    /// <param name="pageIndex">The index of the current page.</param>
    /// <param name="pageSize">The size of the page.</param>
    /// <param name="sortColumn">The column to sort by.</param>
    /// <param name="sortOrder">The sort order.</param>
    /// <param name="filterColumn">The column to filter by.</param>
    /// <param name="filterQuery">The filter query.</param>
    private ApiResult(List<T> data, int count, int pageIndex, int pageSize,
        string? sortColumn, string? sortOrder, string? filterColumn, string? filterQuery)
    {
        Data = data;
        PageIndex = pageIndex;
        PageSize = pageSize;
        TotalCount = count;
        TotalPages = (int)Math.Ceiling(count / (double)PageSize);
        SortColumn = sortColumn;
        SortOrder = sortOrder;
        FilterColumn = filterColumn;
        FilterQuery = filterQuery;
    }

    /// <summary>
    /// Creates an <see cref="ApiResult{T}"/> asynchronously.
    /// </summary>
    /// <param name="source">The source queryable.</param>
    /// <param name="pageIndex">The index of the current page.</param>
    /// <param name="pageSize">The size of the page.</param>
    /// <param name="sortColumn">The column to sort by.</param>
    /// <param name="sortOrder">The sort order.</param>
    /// <param name="filterColumn">The column to filter by.</param>
    /// <param name="filterQuery">The filter query.</param>
    /// <returns>An <see cref="ApiResult{T}"/> containing the paginated data.</returns>
    public static async Task<ApiResult<T>> CreateAsync(
        IQueryable<T> source,
        int pageIndex,
        int pageSize,
        string? sortColumn = null,
        string? sortOrder = null,
        string? filterColumn = null,
        string? filterQuery = null)
    {
        if (!string.IsNullOrEmpty(filterColumn) && (!string.IsNullOrEmpty(filterQuery) && IsValidProperty(filterColumn)))
        {
            source = source.Where(filterColumn, filterQuery);
        }

        var count = await source.CountAsync();

        if (!string.IsNullOrEmpty(sortColumn) && IsValidProperty(sortColumn))
        {
            sortOrder = !string.IsNullOrEmpty(sortOrder) &&
                sortOrder.Equals("ASC", StringComparison.OrdinalIgnoreCase) ? "ASC" : "DESC";

            source = source.OrderBy(sortColumn, sortOrder);
        }

        source = source.Skip(pageIndex * pageSize).Take(pageSize);

        var data = await source.ToListAsync();

        return new ApiResult<T>(data, count, pageIndex, pageSize, sortColumn, sortOrder, filterColumn, filterQuery);
    }

    /// <summary>
    /// Checks if the property name exists to protect against SQL injection attacks.
    /// </summary>
    /// <param name="propertyName">The name of the property to check.</param>
    /// <param name="throwExceptionIfNotFound">Whether to throw an exception if the property is not found.</param>
    /// <returns>True if the property exists; otherwise, false.</returns>
    /// <exception cref="NotSupportedException">Thrown if the property does not exist and <paramref name="throwExceptionIfNotFound"/> is true.</exception>
    private static bool IsValidProperty(string propertyName, bool throwExceptionIfNotFound = true)
    {
        var prop = typeof(T).GetProperty(
            propertyName,
            BindingFlags.IgnoreCase |
            BindingFlags.Public |
            BindingFlags.Instance);

        if (prop == null && throwExceptionIfNotFound)
        {
            throw new NotSupportedException(string.Format($"ERROR: Property '{propertyName}' does not exist."));
        }

        return prop != null;
    }
}
