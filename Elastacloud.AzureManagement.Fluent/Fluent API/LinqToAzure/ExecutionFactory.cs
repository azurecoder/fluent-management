/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Elastacloud.AzureManagement.Fluent.Linq;
using Elastacloud.AzureManagement.Fluent.Types;
using Elastacloud.AzureManagement.Fluent.Types.VirtualMachines;
using Elastacloud.AzureManagement.Fluent.VirtualMachines.Classes;

namespace Elastacloud.AzureManagement.Fluent.LinqToAzure
{
    /// <summary>
    /// A factory method used to derive the return type of the IQueryable and execute the appropriate method
    /// </summary>
    public class ExecutionFactory
    {
        /// <summary>
        /// Lambda expression used taken from the query 
        /// </summary>
        private readonly Expression _expression;
        /// <summary>
        /// The derived and parsed lambda expression
        /// </summary>
        private LambdaExpression _lambdaExpression;
        /// <summary>
        /// The inputs to Windows Azure for subscriptions
        /// </summary>
        private readonly LinqToAzureInputs _inputs;
        /// <summary>
        /// The execute query details interface for storage, cloud and other service types
        /// </summary>
        private IExecuteQuery _queryable;
        /// <summary>
        /// The type returned from the queryable interface 
        /// </summary>
        private Type _expressionQueryableType;

        /// <summary>
        /// Used to construct an ExecutionFactory
        /// </summary>
        /// <param name="expression">The expression reduced from the query</param>
        /// <param name="inputs">The inputs to Windows Azure</param>
        public ExecutionFactory(Expression expression, LinqToAzureInputs inputs)
        {
            _expression = expression;
            _inputs = inputs;
        }

        /// <summary>
        /// Used to derive the lambda expression and determine whether the query is correct
        /// </summary>
        private void ExtrapolateLambdas()
        {
            var methodCallExpression = _expression as MethodCallExpression;
            if (methodCallExpression == null && methodCallExpression.Arguments.Count == 0)
                throw new InvalidQueryException("unable to execute query ensure that expression is a Linq expression");

            var argumentExpression = methodCallExpression.Arguments[0] as MethodCallExpression;
            if (argumentExpression == null && argumentExpression.Arguments.Count == 0)
                throw new InvalidQueryException("unable to execute query ensure that expression is a Linq expression");
            // get the first argument of the expression which should be focussed on the initial 
            var typeExpression = argumentExpression.Arguments[0] as ConstantExpression;
            if (typeExpression == null)
                throw new InvalidQueryException("unable to execute query ensure that expression is a Linq expression");

            _expressionQueryableType = typeExpression.Type;

            var whereFinder = new InnermostWhereFinder();
            MethodCallExpression whereExpression = whereFinder.GetInnermostWhere(_expression);
            // if we don't have a where clause we only want to evaluate the first part of the expression
            if (whereExpression == null)
                return;

            var lambdaExpression = (LambdaExpression)((UnaryExpression)(whereExpression.Arguments[1])).Operand;

            // Send the lambda expression through the partial evaluator.
            _lambdaExpression = (LambdaExpression)Evaluator.PartialEval(lambdaExpression);
        }

        /// <summary>
        /// Gets the concrete class type as a queryable
        /// </summary>
        /// <returns>An IQueryable interface</returns>
        public IQueryable GetConcreteQueryable()
        {
            ExtrapolateLambdas();
            if (_expressionQueryableType == typeof(LinqToAzureOrderedQueryable<StorageAccount>))
            {
                 _queryable = new StorageExecutor(_lambdaExpression);
                return _queryable.Execute<StorageAccount>(_inputs);
            }
            if (_expressionQueryableType == typeof(LinqToAzureOrderedQueryable<CloudService>))
            {
                _queryable = new CloudServiceExecutor(_lambdaExpression);
                return _queryable.Execute<CloudService>(_inputs);
            }
            if (_expressionQueryableType == typeof(LinqToAzureOrderedQueryable<VirtualMachineProperties>))
            {
                _queryable = new VirtualMachineExecutor(_lambdaExpression);
                return _queryable.Execute<List<PersistentVMRole>>(_inputs);
            }
            throw new ApplicationException("Unsupported factory type");
        }
    }
}
