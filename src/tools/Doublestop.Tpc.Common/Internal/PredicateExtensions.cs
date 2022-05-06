using System.Linq.Expressions;

namespace Doublestop.Tpc.Internal;

internal static class PredicateExtensions
{
    internal static Expression<Func<T, bool>> True<T>() => _ => true;

    internal static Expression<Func<T, bool>> And<T>(
        this Expression<Func<T, bool>> expr1,
        Expression<Func<T, bool>> expr2)
    {
        // need to detect whether they use the same
        // parameter instance; if not, they need fixing
        var param = expr1.Parameters[0];
        if (ReferenceEquals(param, expr2.Parameters[0]))
            // simple version
            return Expression.Lambda<Func<T, bool>>(
                Expression.AndAlso(expr1.Body, expr2.Body), param);

        // otherwise, keep expr1 "as is" and invoke expr2
        return Expression.Lambda<Func<T, bool>>(
            Expression.AndAlso(
                expr1.Body,
                Expression.Invoke(expr2, param)), param);
    }

    internal static Expression<Func<T, bool>> And<T>(Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2, params Expression<Func<T, bool>>[] more) => 
        JoinAll(more.Prepend(expr2).Prepend(expr1));

    internal static Expression<Func<T, bool>> JoinAll<T>(
        this IEnumerable<Expression<Func<T, bool>>> expressions)
    {
        Expression<Func<T, bool>>? joined = null;
        foreach (var e in expressions)
        {
            if (joined is null)
            {
                joined = e;
                continue;
            }
            joined = joined.And(e);
        }
        return joined ?? True<T>();
    }

    internal static Expression<Func<T, bool>> Or<T>(Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2, params Expression<Func<T, bool>>[] more) => 
        JoinOr(more.Prepend(expr2).Prepend(expr1));

    internal static Expression<Func<T, bool>> JoinOr<T>(this IEnumerable<Expression<Func<T, bool>>> expressions)
    {
        Expression<Func<T, bool>>? joined = null;
        foreach (var e in expressions)
        {
            if (joined is null)
            {
                joined = e;
                continue;
            }
            joined = joined.Or(e);
        }
        return joined ?? True<T>();
    }

    internal static Expression<Func<T, bool>> Or<T>(
        this Expression<Func<T, bool>> expr1,
        Expression<Func<T, bool>> expr2)
    {
        // need to detect whether they use the same
        // parameter instance; if not, they need fixing
        var param = expr1.Parameters[0];
        if (ReferenceEquals(param, expr2.Parameters[0]))
            // simple version
            return Expression.Lambda<Func<T, bool>>(
                Expression.Or(expr1.Body, expr2.Body), param);

        // otherwise, keep expr1 "as is" and invoke expr2
        return Expression.Lambda<Func<T, bool>>(
            Expression.Or(
                expr1.Body,
                Expression.Invoke(expr2, param)), param);
    }
}