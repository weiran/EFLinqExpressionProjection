using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Collections;
using System.Data.Entity.Infrastructure;

namespace LinqExpressionProjection
{
    /// <summary>
    /// An IQueryable wrapper that allows us to visit the query's expression tree just before LINQ to SQL gets to it.
    /// This is based on the excellent work of Tomas Petricek: http://tomasp.net/blog/linq-expand.aspx
    /// </summary>
    public class ProjectionSupportingQuery<T> : IQueryable<T>, IOrderedQueryable<T>, IOrderedQueryable, IDbAsyncEnumerable<T>
    {
        ProjectionSupportingQueryProvider<T> _provider;
		IQueryable<T> _inner;

		internal IQueryable<T> InnerQuery { get { return _inner; } }			// Original query, that we're wrapping

        internal ProjectionSupportingQuery(IQueryable<T> inner)
		{
			_inner = inner;
            _provider = new ProjectionSupportingQueryProvider<T>(this);
		}

		Expression IQueryable.Expression { get { return _inner.Expression; } }
		Type IQueryable.ElementType { get { return typeof (T); } }
		IQueryProvider IQueryable.Provider { get { return _provider; } }
		public IEnumerator<T> GetEnumerator () { return _inner.GetEnumerator (); }
		IEnumerator IEnumerable.GetEnumerator () { return _inner.GetEnumerator (); }
		public override string ToString () { return _inner.ToString (); }
        IDbAsyncEnumerator<T> IDbAsyncEnumerable<T>.GetAsyncEnumerator() { return new AsyncEnumerator<T>(_inner.AsEnumerable().GetEnumerator()); }
        public IDbAsyncEnumerator GetAsyncEnumerator() { return GetAsyncEnumerator(); }
    }


}
