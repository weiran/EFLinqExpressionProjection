using System.Collections.Generic;
using System.Linq.Expressions;

namespace LinqExpressionProjection
{
    public class ParameterRebinder : ExpressionVisitor
    {
        private readonly Dictionary<ParameterExpression, Expression> map;

        public ParameterRebinder(Dictionary<ParameterExpression, Expression> map)
        {
            this.map = map ?? new Dictionary<ParameterExpression, Expression>();
        }

        protected override Expression VisitParameter(ParameterExpression parameter)
        {
            return map.ContainsKey(parameter) ? Visit(map[parameter]) : base.VisitParameter(parameter);
        }
    }
}
