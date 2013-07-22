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
using Elastacloud.AzureManagement.Fluent.Types.VirtualMachines;
using Elastacloud.AzureManagement.Fluent.VirtualMachines.Classes;

namespace Elastacloud.AzureManagement.Fluent.Linq
{
    /// <summary>
    /// Finds a virtual machine within a cloud service deployment
    /// </summary>
    internal class VirtualMachineFinder : ExpressionVisitor
    {
        private readonly Expression _expression;
        private string _cloudServiceName;

        public VirtualMachineFinder(Expression exp)
        {
            this._expression = exp;
        }

        public string CloudServiceName
        {
            get
            {
                if (_cloudServiceName == null)
                {
                    _cloudServiceName = string.Empty;
                    if(_expression != null)
                        this.Visit(this._expression);
                }
                return this._cloudServiceName;
            }
        }

        protected override Expression VisitBinary(BinaryExpression be)
        {
            if (be.NodeType == ExpressionType.Equal)
            {
                if (ExpressionTreeHelpers.IsMemberEqualsValueExpression(be, typeof(VirtualMachineProperties), "CloudServiceName"))
                {
                    _cloudServiceName = ExpressionTreeHelpers.GetValueFromEqualsExpression(be, typeof(VirtualMachineProperties), "CloudServiceName");
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