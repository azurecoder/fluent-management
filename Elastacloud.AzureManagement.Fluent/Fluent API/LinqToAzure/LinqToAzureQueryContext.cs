using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Elastacloud.AzureManagement.Fluent.Types;

namespace Elastacloud.AzureManagement.Fluent.Linq
{
    public class LinqToAzureQueryContext
    {
        // Executes the expression tree that is passed to it. 
        internal static object Execute(Expression expression, bool isEnumerable, LinqToAzureInputs inputs)
        {
            // The expression must represent a query over the data source. 
            if (!IsQueryOverDataSource(expression))
                throw new InvalidProgramException("No query over the data source was specified.");

            // Find the call to Where() and get the lambda expression predicate.
            var whereFinder = new InnermostWhereFinder();
            MethodCallExpression whereExpression = whereFinder.GetInnermostWhere(expression);
            var lambdaExpression = (LambdaExpression)((UnaryExpression)(whereExpression.Arguments[1])).Operand;

            // Send the lambda expression through the partial evaluator.
            lambdaExpression = (LambdaExpression)Evaluator.PartialEval(lambdaExpression);

            // Get the place name(s) to query the Web service with.
            var sf = new StorageFinder(lambdaExpression.Body);
            var storageAccounts = sf.StorageAccounts;
            if (storageAccounts.Count == 0)
                throw new InvalidQueryException("You must specify at least one place name in your query.");

            // Call the Web service and get the results.
            var manager = new SubscriptionManager(inputs.SubscriptionId);
            var storageManager = manager.GetStorageManager();
            var accounts = storageManager.ForStorageInformationQuery()
                .AddCertificateFromStore(inputs.ManagementCertificateThumbprint)
                .GetStorageAccountList(true);

            // Copy the IEnumerable places to an IQueryable.
            IQueryable<StorageAccount> queryableStorageAccount = accounts.AsQueryable();

            // Copy the expression tree that was passed in, changing only the first 
            // argument of the innermost MethodCallExpression.
            var treeCopier = new ExpressionTreeModifier(queryableStorageAccount);
            var newExpressionTree = treeCopier.Visit(expression);

            // This step creates an IQueryable that executes by replacing Queryable methods with Enumerable methods. 
            if (isEnumerable)
                return queryableStorageAccount.Provider.CreateQuery(newExpressionTree);
            else
                return queryableStorageAccount.Provider.Execute(newExpressionTree);
        }

        private static bool IsQueryOverDataSource(Expression expression)
        {
            // If expression represents an unqueried IQueryable data source instance, 
            // expression is of type ConstantExpression, not MethodCallExpression. 
            return (expression is MethodCallExpression);
        }

    }
}