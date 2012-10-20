using System.Collections.Generic;
using System.Linq.Expressions;
using Elastacloud.AzureManagement.Fluent.Types;

namespace Elastacloud.AzureManagement.Fluent.Linq
{
    internal class StorageFinder : ExpressionVisitor
    {
        private readonly Expression _expression;
        private List<string> _storageAccounts;

        public StorageFinder(Expression exp)
        {
            this._expression = exp;
        }

        public List<string> StorageAccounts
        {
            get
            {
                if (_storageAccounts == null)
                {
                    _storageAccounts = new List<string>();
                    this.Visit(this._expression);
                }
                return this._storageAccounts;
            }
        }

        protected override Expression VisitBinary(BinaryExpression be)
        {
            if (be.NodeType == ExpressionType.Equal)
            {
                if (ExpressionTreeHelpers.IsMemberEqualsValueExpression(be, typeof(StorageAccount), "Name"))
                {
                    _storageAccounts.Add(ExpressionTreeHelpers.GetValueFromEqualsExpression(be, typeof(StorageAccount), "Name"));
                    return be;
                }
                else if (ExpressionTreeHelpers.IsMemberEqualsValueExpression(be, typeof(StorageAccount), "Url"))
                {
                    _storageAccounts.Add(ExpressionTreeHelpers.GetValueFromEqualsExpression(be, typeof(StorageAccount), "Url"));
                    return be;
                }
                else 
                    return base.VisitBinary(be);
            }
            else 
                return base.VisitBinary(be);
        }
    }
}