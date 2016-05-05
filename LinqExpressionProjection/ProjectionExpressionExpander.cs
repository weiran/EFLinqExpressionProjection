using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LinqExpressionProjection
{
    /// <summary>
    /// Custom expresssion visitor for ExpandableQuery. This expands calls to Expression.Compile() and
    /// collapses captured lambda references in subqueries which LINQ to SQL can't otherwise handle.
    /// </summary>
    internal class ProjectionExpressionExpander : ExpressionVisitor
    {
        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m.Method.Name == "Project" && m.Method.DeclaringType == typeof(Extensions))
            {
                Expression projectionExpression = m.Arguments[0];

                LambdaExpression projectionLambda = ExtractLambda(projectionExpression);

                // Visit the lambda, essentially extracting the lambda's expression tree
                Expression visitedMethodCall = Visit(projectionLambda.Body);

                // Revisit the lambda's expression tree, replacing the lambda's parameter with the actual expression that results in the same return value
                var parameterMap = new Dictionary<ParameterExpression, Expression>();

                int lambdaParameterArgumentIndex = 1;
                foreach (var parameter in projectionLambda.Parameters)
                {
                    parameterMap.Add(parameter, m.Arguments[lambdaParameterArgumentIndex++]);
                }

                visitedMethodCall = new ParameterRebinder(parameterMap).Visit(visitedMethodCall);
                return visitedMethodCall;
            }

            return base.VisitMethodCall(m);
        }

        /// <summary>
        /// Expects the specified projectionExpression to return a lambda expression.
        /// This method creates a lambda expression, whose body is the expression that returns the actual lambda projection expression,
        /// and executes the wrapping lambda expression to return the actual lambda projection expression. 
        /// </summary>
        private static LambdaExpression ExtractLambda(Expression projectionExpression)
        {
            return Expression.Lambda<Func<LambdaExpression>>(projectionExpression).Compile().Invoke();
        }
    }
}
