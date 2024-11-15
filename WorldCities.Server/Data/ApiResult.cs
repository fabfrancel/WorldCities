using Microsoft.EntityFrameworkCore;
using System.Reflection;
using WorldCities.Server.Extensions;

namespace WorldCities.Server.Data;

public class ApiResult<T>
{
    public List<T> Data { get; private set; }
    public int PageIndex { get; private set; }
    public int PageSize { get; private set; }
    public int TotalCount { get; private set; }
    public int TotalPages { get; private set; }

    public bool HasPreviousPage => PageIndex > 0;
    public bool HasNextPage => PageIndex + 1 < TotalPages;

    public string? SortColumn { get; set; }
    public string? SortOrder { get; set; }

    public string? FilterColumn { get; set; }
    public string? FilterQuery { get; set; }


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
    /// Verifica se o nome da propriedade existe. Para proteger contra SQL injection attacks
    /// </summary>
    /// <param name="propertyName">Nome da propriedade que se deseja verificar se existe</param>
    /// <param name="throwExceptionIfNotFound"></param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
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
