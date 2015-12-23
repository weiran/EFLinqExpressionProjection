using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Data.Entity.Infrastructure;
using System.Threading.Tasks;

namespace LinqExpressionProjection
{
    internal class AsyncQueryProviderWrapper : IDbAsyncQueryProvider
    {
        private readonly IQueryProvider _inner;

        internal AsyncQueryProviderWrapper(IQueryProvider inner)
        {
            _inner = inner;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return _inner.CreateQuery(expression);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return _inner.CreateQuery<TElement>(expression);
        }

        public object Execute(Expression expression)
        {
            return _inner.Execute(expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return _inner.Execute<TResult>(expression);
        }

        public Task<object> ExecuteAsync(Expression expression, CancellationToken cancellationToken)
        {
            return Task.FromResult(Execute(expression));
        }

        public Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
        {
            return Task.FromResult(Execute<TResult>(expression));
        }
    }

}
