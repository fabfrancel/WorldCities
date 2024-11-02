using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;

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

    public string SortColumn { get; set; }
    public string SortOrder { get; set; }


    private ApiResult(List<T> data, int count, int pageIndex, int pageSize, string? sortColumn, string? sortOrder)
    {
        Data = data;
        PageIndex = pageIndex;
        PageSize = pageSize;
        TotalCount = count;
        TotalPages = (int)Math.Ceiling(count / (double)PageSize);
        SortColumn = sortColumn;
        SortOrder = sortOrder;
    }

    public static async Task<ApiResult<T>> CreateAsync(
        IQueryable<T> source,
        int pageIndex,
        int pageSize,
        string? sortColumn = null,
        string? sortOrder = null)
    {
        var count = await source.CountAsync();

        if (!string.IsNullOrEmpty(sortColumn) && IsValidProperty(sortColumn))
        {
            sortOrder = !string.IsNullOrEmpty(sortOrder) && sortOrder.ToUpper() == "ASC" ? "ASC" : "DESC";
            
            source = source.OrderBy(sortColumn, sortOrder);
        }

        source = source.Skip(pageIndex * pageSize).Take(pageSize);
        
        var data = await source.ToListAsync();

        return new ApiResult<T>(data, count, pageIndex, pageSize, sortColumn, sortOrder);
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
           BindingFlags.Instance );

        if (prop == null && throwExceptionIfNotFound)
        {
            throw new NotSupportedException(string.Format($"ERROR: Property '{propertyName}' does not exist."));
        }

        return prop != null;
    }    
}

public static class IQueryableExtensions
{
    /// <summary>
    /// Extensão de método usado para adicionar o método OrderBy personalizado à interface IQueryable<T>. 
    /// Ordena uma lista IQueryable<T> pela propriedade e na direção dadas.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="orderByProperty">A propriedade pela qual se deseja ordenar a lista.</param>
    /// <param name="direction">ASC ou DESC</param>
    /// <returns></returns>
    public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, string orderByProperty, string direction)
    {
        var type = typeof(T);
        var property = type.GetProperty(orderByProperty);
        var parameter = Expression.Parameter(type, "p");
        var propertyAccess = Expression.MakeMemberAccess(parameter, property);
        var orderByExpression = Expression.Lambda(propertyAccess, parameter);

        var methodName = direction == "DESC" ? "OrderByDescending" : "OrderBy";
        var resultExpression = Expression.Call(
            typeof(Queryable), methodName,
            [type, property.PropertyType],
            source.Expression, Expression.Quote(orderByExpression));

        return source.Provider.CreateQuery<T>(resultExpression);
    }
}

/*
 * Extensões de método são uma maneira elegante de adicionar funcionalidades a 
 * tipos existentes sem modificá-los diretamente.
 */
