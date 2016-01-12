using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LinqExpressionProjection
{
    /// <summary>
    /// Custom expresssion visitor for ExpandableQuery. This expands calls to Expression.Compile() and
    /// collapses captured lambda references in subqueries which LINQ to SQL can't otherwise handle.
    /// </summary>
    class ProjectionExpressionExpander : ExpressionVisitor
    {
        readonly Stack<MethodCallExpression> _selectStack = new Stack<MethodCallExpression>();

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            Expression visitedMethodCall = null;

            if (m.Method.Name == "Select")
            {
                _selectStack.Push(m);
            }

            if (m.Method.Name == "Project" && m.Method.DeclaringType == typeof(Extensions))
            {
                // Project() is expected to be invoked on an Expression<Func<In, Out>>,
                // the expression can be contained in a local variable, a field, retrived from a method
                // or any other expression tree as long as it can be compiled and executed to return a proper lambda
                ParameterExpression selectParameter = GetSelectParameter(_selectStack.Peek());
                LambdaExpression innerLambda = GetInnerLambda(m.Arguments[0], selectParameter.Type, m.Method.ReturnType);

                // By returning the visited body of the lambda we ommit the call to Project()
                // and the tree producing the selector lambda:
                visitedMethodCall = Visit(innerLambda.Body);

                // The lambda takes a parameter, this parameter must be replaced by the parameter provided for the select lambda
                // Rebind parameter:
                var map = new Dictionary<ParameterExpression, Expression>
                          {
                              { innerLambda.Parameters[0], selectParameter }
                          };
                visitedMethodCall = new ParameterRebinder(map).Visit(visitedMethodCall);
            }

            if (m.Method.Name == "Project2" && m.Method.DeclaringType == typeof(Extensions))
            {
                // rs-todo: next line could possibly be used to work out ParameterExpression for member access of expressions?
                //bool isInstanceContext = (m.Arguments[0] as MemberExpression)?.Expression != null;

                Expression projectionExpression = m.Arguments[0];

                LambdaExpression projectionLambda = ExtractLambda(projectionExpression);

                // Visit the lambda, essentially exctracting the lambda's expression tree
                visitedMethodCall = Visit(projectionLambda.Body);

                // Revisit the lambda's expression tree, replacing the lambda's parameter with the actual expression that results in the same return value
                Expression lambdaParameterReplacement = m.Arguments[1];
                var map = new Dictionary<ParameterExpression, Expression>
                          {
                              { projectionLambda.Parameters[0], lambdaParameterReplacement }
                          };
                visitedMethodCall = new ParameterRebinder(map).Visit(visitedMethodCall);
            }

            if (visitedMethodCall == null)
            {
                visitedMethodCall = base.VisitMethodCall(m);
            }

            if (m.Method.Name == "Select")
            {
                _selectStack.Pop();
            }
            return visitedMethodCall;
        }

        private static LambdaExpression ExtractLambda(Expression projectionExpression)
        {
            return Expression.Lambda<Func<LambdaExpression>>(projectionExpression).Compile().Invoke();
        }

        /// <summary>
        /// This method executes expression and expects it to return a lambda expression taking paramter of 
        /// certain type and returning a value of certain type.
        /// </summary>
        private static LambdaExpression GetInnerLambda(Expression projectionExpression, Type parameterType, Type returnType)
        {
            Exception innerException = null;
            try
            {
                Expression<Func<LambdaExpression>> executionLambda = Expression.Lambda<Func<LambdaExpression>>(projectionExpression);
                LambdaExpression extractedLambda = executionLambda.Compile().Invoke();
                if (extractedLambda != null
                    && extractedLambda.Parameters[0].Type == parameterType
                    && extractedLambda.ReturnType == returnType)
                {
                    return extractedLambda;
                }
            }
            catch (Exception e)
            {
                innerException = e;
            }
            throw new InvalidOperationException(string.Format("Lambda expression with parameter of type '{0}' and return type '{1}' was not located after Project() call ({2})", parameterType, returnType, projectionExpression), innerException);
        }

        private ParameterExpression GetSelectParameter(MethodCallExpression selectExpression)
        {
            LambdaExpression selectionLambda = SkipUnwantedExpressions(selectExpression.Arguments[1]) as LambdaExpression;
            if (selectionLambda != null)
            {
                return selectionLambda.Parameters[0];
            }
            throw new InvalidOperationException(string.Format("Lambda not found in select expression '{0}'", selectExpression));
        }

        private static Expression SkipUnwantedExpressions(Expression expression)
        {
            if (expression is UnaryExpression)
            {
                return SkipUnwantedExpressions(((UnaryExpression)expression).Operand);
            }
            return expression;
        }
    }
}
