/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Elastacloud.AzureManagement.Fluent.Linq;
using Elastacloud.AzureManagement.Fluent.Types;

namespace Elastacloud.AzureManagement.Fluent.LinqToAzure
{
    internal class CloudServiceFinder : ExpressionVisitor
    {
        private readonly Expression _expression;
        private List<string> _cloudServiceNames;

        public CloudServiceFinder(Expression exp)
        {
            this._expression = exp;
        }

        public List<string> CloudServiceNames
        {
            get
            {
                if (_cloudServiceNames == null)
                {
                    _cloudServiceNames = new List<string>();
                    if(_expression != null)
                    this.Visit(this._expression);
                }
                return this._cloudServiceNames;
            }
        }

        protected override Expression VisitBinary(BinaryExpression be)
        {
            if (be.NodeType == ExpressionType.Equal)
            {
                if (ExpressionTreeHelpers.IsMemberEqualsValueExpression(be, typeof (CloudService), "Name"))
                {
                    _cloudServiceNames.Add(ExpressionTreeHelpers.GetValueFromEqualsExpression(be, typeof (CloudService),
                                                                                              "Name"));
                    return be;
                }
                else if (ExpressionTreeHelpers.IsMemberEqualsValueExpression(be, typeof (CloudService), "Url"))
                {
                    _cloudServiceNames.Add(ExpressionTreeHelpers.GetValueFromEqualsExpression(be, typeof (CloudService),
                                                                                              "Url"));
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
