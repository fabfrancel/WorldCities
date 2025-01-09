using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

/*
 * Extensões de método são uma maneira elegante de adicionar funcionalidades a 
 * tipos existentes sem modificá-los diretamente.
 */


namespace WorldCities.Server.Extensions;

/// <summary>
/// Provides extension methods for IQueryable.
/// </summary>
public static class IQueryableExtensions
{
    /// <summary>
    /// Specifies the filter methods available for string properties.
    /// </summary>
    public enum FilterMethods
    {
        StartsWith,
        Contains,
        EndsWith
    }

    /// <summary>
    /// Orders the elements of a sequence according to a specified property and direction.
    /// </summary>
    /// <example>
    /// source.OrderBy("name", "ASC")
    /// </example>
    /// <remarks>
    /// original OrderBy method signature: public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, Expression<Func<T, object>> keySelector)
    /// original OrderBy method example: cities = cities.OrderBy(c => c.Name);
    /// </remarks>  
    /// <typeparam name="T">The type of the elements of source.</typeparam>
    /// <param name="source">An IQueryable&lt;T&gt; to order.</param>
    /// <param name="orderByProperty">The name of the property to order by.</param>
    /// <param name="direction">The direction to order by ("ASC" or "DESC").</param>
    /// <returns>An IQueryable<T> whose elements are sorted according to the specified property and direction.</returns>
    public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, string orderByProperty, string direction)
    {
        var type = typeof(T);
        var property = type.GetProperty(
            orderByProperty,
            BindingFlags.IgnoreCase |
            BindingFlags.Public |
            BindingFlags.Instance);
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

    /// <summary>
    /// Filters the elements of a sequence based on a specified property and query.
    /// </summary>
    /// <example>
    /// source.Where("name", "John")
    /// </example>
    /// <remarks>
    /// original Where method signature: public static IQueryable<T> Where<T>(this IQueryable<T> source, Expression<Func<T, bool>> predicate)
    /// original Where method example: cities = cities.Where(c => c.Name.StartsWith("San"));
    /// </remarks>
    /// <typeparam name="T">The type of the elements of source.</typeparam>
    /// <param name="source">An IQueryable&lt;T&gt; to filter.</param>
    /// <param name="filterColumn">The name of the property to filter by.</param>
    /// <param name="filterQuery">The query to filter by.</param>
    /// <param name="filterMethodName">The filter method to use (StartsWith, Contains, EndsWith).</param>
    /// <returns>An IQueryable<T> that contains elements from the input sequence that satisfy the condition specified by filterColumn and filterQuery.</returns>
    public static IQueryable<T> Where<T>(this IQueryable<T> source, string filterColumn, string filterQuery, FilterMethods filterMethodName = FilterMethods.StartsWith)
    {
        var type = typeof(T);
        var property = type.GetProperty(
            filterColumn,
            BindingFlags.IgnoreCase |
            BindingFlags.Public |
            BindingFlags.Instance);
        var parameter = Expression.Parameter(type, "p");
        var propertyAccess = Expression.MakeMemberAccess(parameter, property);
        
        var constant = Expression.Constant(filterQuery);
        var filterMethod = typeof(string).GetMethod(filterMethodName.ToString(), [typeof(string)]);
        var body = Expression.Call(propertyAccess, filterMethod, constant);
        
        var lambdaExpression = Expression.Lambda<Func<T, bool>>(body, parameter);

        var resultExpression = Expression.Call(
            typeof(Queryable), "Where",
            [type],
            source.Expression, Expression.Quote(lambdaExpression));

        return source.Provider.CreateQuery<T>(resultExpression);
    }
}
