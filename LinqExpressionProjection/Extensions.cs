using System;
using System.Linq;
using System.Linq.Expressions;

namespace LinqExpressionProjection
{
    public static class Extensions
    {
        public static IQueryable<T> AsExpressionProjectable<T>(this IQueryable<T> query)
        {
            if (query is ProjectionSupportingQuery<T>) return (ProjectionSupportingQuery<T>)query;
            return new ProjectionSupportingQuery<T>(query);
        }

        public static T Project<T>(this LambdaExpression expr)
        {
            throw new NotSupportedException("'Project()' method cannot be invoked. Call 'AsExpressionProjectable()' on the collection being queried.");
        }

        public static TResult Project2<TIn, TResult>(this Expression<Func<TIn, TResult>> expr, TIn input)
        {
            throw new NotSupportedException("'Project()' method cannot be invoked. Call 'AsExpressionProjectable()' on the collection being queried.");
        }

        public static Expression ExpandExpressionsForProjection(this Expression expr)
        {
            Expression projectionsCorrected = new ProjectionExpressionExpander().Visit(expr);
            return projectionsCorrected;
        }
    }
}
