using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

/*
 * Extensões de método são uma maneira elegante de adicionar funcionalidades a 
 * tipos existentes sem modificá-los diretamente.
 */


namespace WorldCities.Server.Extensions;

public static class IQueryableExtensions
{
    public enum FilterMethods
    {
        StartsWith,
        Contains,
        EndsWith
    }


    // source.OrderBy("name", "ASC")
    // original OrderBy method signature: public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, Expression<Func<T, object>> keySelector)
    // original OrderBy method example: cities = cities.OrderBy(c => c.Name);
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

    // source.Where("name", "John")
    // original Where method signature: public static IQueryable<T> Where<T>(this IQueryable<T> source, Expression<Func<T, bool>> predicate)
    // original Where method example: cities = cities.Where(c => c.Name.StartsWith("San"));
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
