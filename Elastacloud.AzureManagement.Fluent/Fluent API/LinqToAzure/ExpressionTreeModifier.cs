using System.Linq;
using System.Linq.Expressions;
using Elastacloud.AzureManagement.Fluent.Types;

namespace Elastacloud.AzureManagement.Fluent.Linq
{
    internal class ExpressionTreeModifier : ExpressionVisitor
    {
        private readonly IQueryable<StorageAccount> _storageAccounts;

        internal ExpressionTreeModifier(IQueryable<StorageAccount> accounts)
        {
            this._storageAccounts = accounts;
        }

        protected override Expression VisitConstant(ConstantExpression c)
        {
            // Replace the constant QueryableTerraServerData arg with the queryable Place collection. 
            if (c.Type == typeof (LinqToAzureOrderedQueryable<StorageAccount>))
                return Expression.Constant(_storageAccounts);
            else
                return c;
        }
    }
}