using System;
using System.Linq;
using System.Linq.Expressions;

// ReSharper disable UnusedParameter.Global

namespace LinqExpressionProjection
{
    public static class Extensions
    {
        public static IQueryable<T> AsExpressionProjectable<T>(this IQueryable<T> query)
        {
            if (query is ProjectionSupportingQuery<T>)
            {
                return (ProjectionSupportingQuery<T>)query;
            }
            return new ProjectionSupportingQuery<T>(query);
        }

        public static TResult Project<TIn, TResult>(this Expression<Func<TIn, TResult>> expressionToProject, TIn inputToExpressionToProject)
        {
            throw new NotSupportedException("'Project()' method cannot be invoked. Call 'AsExpressionProjectable()' on the collection being queried.");
        }

        public static TResult Project<TIn1, TIn2, TResult>(
            this Expression<Func<TIn1, TIn2, TResult>> expressionToProject,
            TIn1 input1ToExpressionToProject,
            TIn2 input2ToExpressionToProject)
        {
            throw new NotSupportedException("'Project()' method cannot be invoked. Call 'AsExpressionProjectable()' on the collection being queried.");
        }

        public static TResult Project<TIn1, TIn2, TIn3, TResult>(
            this Expression<Func<TIn1, TIn2, TIn3, TResult>> expressionToProject,
            TIn1 input1ToExpressionToProject,
            TIn2 input2ToExpressionToProject,
            TIn3 input3ToExpressionToProject)
        {
            throw new NotSupportedException("'Project()' method cannot be invoked. Call 'AsExpressionProjectable()' on the collection being queried.");
        }

        public static TResult Project<TIn1, TIn2, TIn3, TIn4, TResult>(
            this Expression<Func<TIn1, TIn2, TIn3, TIn4, TResult>> expressionToProject,
            TIn1 input1ToExpressionToProject,
            TIn2 input2ToExpressionToProject,
            TIn3 input3ToExpressionToProject,
            TIn4 input4ToExpressionToProject)
        {
            throw new NotSupportedException("'Project()' method cannot be invoked. Call 'AsExpressionProjectable()' on the collection being queried.");
        }

        public static TResult Project<TIn1, TIn2, TIn3, TIn4, TIn5, TResult>(
            this Expression<Func<TIn1, TIn2, TIn3, TIn4, TIn5, TResult>> expressionToProject,
            TIn1 input1ToExpressionToProject,
            TIn2 input2ToExpressionToProject,
            TIn3 input3ToExpressionToProject,
            TIn4 input4ToExpressionToProject,
            TIn5 input5ToExpressionToProject)
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
