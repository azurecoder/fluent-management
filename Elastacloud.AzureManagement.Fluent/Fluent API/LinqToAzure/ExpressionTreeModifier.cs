/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/
using System.Linq;
using System.Linq.Expressions;
using Elastacloud.AzureManagement.Fluent.Types;

namespace Elastacloud.AzureManagement.Fluent.Linq
{
    internal class ExpressionTreeModifier : ExpressionVisitor
    {
        private readonly IQueryable _accounts;

        internal ExpressionTreeModifier(IQueryable accounts)
        {
            this._accounts = accounts;
        }

        protected override Expression VisitConstant(ConstantExpression c)
        {
            // have to do something here about type matching the types of Azure services
            if (c.Type == typeof(LinqToAzureOrderedQueryable<StorageAccount>) || c.Type == typeof(LinqToAzureOrderedQueryable<CloudService>))
                return Expression.Constant(_accounts);
            return c;
        }
    }
}