using System.Linq.Expressions;

namespace Elastacloud.AzureManagement.Fluent.Linq
{
    internal class InnermostWhereFinder : ExpressionVisitor
    {
        private MethodCallExpression _innermostWhereExpression;

        public MethodCallExpression GetInnermostWhere(Expression expression)
        {
            Visit(expression);
            return _innermostWhereExpression;
        }

        protected override Expression VisitMethodCall(MethodCallExpression expression)
        {
            if (expression.Method.Name == "Where")
                _innermostWhereExpression = expression;

            Visit(expression.Arguments[0]);

            return expression;
        }
    }
}