/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/
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
                    if(_expression != null)
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