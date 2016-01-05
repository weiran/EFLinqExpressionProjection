using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace LinqExpressionProjection
{
    internal class ProjectionSupportingQueryProvider<T> : IQueryProvider, IDbAsyncQueryProvider
    {
        private readonly ProjectionSupportingQuery<T> _query;

        internal ProjectionSupportingQueryProvider(ProjectionSupportingQuery<T> query)
        {
            _query = query;
        }

        // The following four methods first call ExpressionExpander to visit the expression tree, then call
        // upon the inner query to do the remaining work.

        IQueryable<TElement> IQueryProvider.CreateQuery<TElement>(Expression expression)
        {
            return new ProjectionSupportingQuery<TElement>(_query.InnerQuery.Provider.CreateQuery<TElement>(expression.ExpandExpressionsForProjection()));
        }

        IQueryable IQueryProvider.CreateQuery(Expression expression)
        {
            return _query.InnerQuery.Provider.CreateQuery(expression.ExpandExpressionsForProjection());
        }

        object IQueryProvider.Execute(Expression expression)
        {
            return _query.InnerQuery.Provider.Execute(expression.ExpandExpressionsForProjection());
        }

        TResult IQueryProvider.Execute<TResult>(Expression expression)
        {
            return _query.InnerQuery.Provider.Execute<TResult>(expression.ExpandExpressionsForProjection());
        }

        Task<object> IDbAsyncQueryProvider.ExecuteAsync(Expression expression, CancellationToken cancellationToken)
        {
            return Task.FromResult(_query.InnerQuery.Provider.Execute(expression.ExpandExpressionsForProjection()));
        }

        Task<TResult> IDbAsyncQueryProvider.ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
        {
            return Task.FromResult(_query.InnerQuery.Provider.Execute<TResult>(expression.ExpandExpressionsForProjection()));
        }
    }
}
