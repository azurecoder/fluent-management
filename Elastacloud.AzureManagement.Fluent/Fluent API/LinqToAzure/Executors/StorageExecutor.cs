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

namespace Elastacloud.AzureManagement.Fluent.LinqToAzure
{
    /// <summary>
    /// Used to execute a lambda expression against storage services
    /// </summary>
    public class StorageExecutor : IExecuteQuery
    {
        /// <summary>
        /// The preparsed lambda expression for use with it's body
        /// </summary>
        private readonly LambdaExpression _expression;

        /// <summary>
        /// Constructs a StorageExecutor object
        /// </summary>
        /// <param name="expression">Used to construct the lambda expression for storage</param>
        public StorageExecutor(LambdaExpression expression)
        {
            _expression = expression;
        }

        #region Implementation of IExecuteQuery

        /// <summary>
        /// Executes the query performing the underlying operation
        /// </summary>
        /// <typeparam name="T">The type used to define the query - CloudService or StorageAccount</typeparam>
        /// <param name="inputs">The Authentication inputs needed to fulfil the operation</param>
        /// <returns>An IQueryable interface</returns>
        public IQueryable<T> Execute<T>(LinqToAzureInputs inputs)
        {
            if (typeof(T) != GetValidType())
                throw new InvalidQueryException("Mismatch between generic types StorageAccount type expected");

            // Get the place name(s) to query the Web service with.
            var sf = new StorageFinder(_expression != null ? _expression.Body : null);
            var storageAccounts = sf.StorageAccounts;
            //if (storageAccounts.Count == 0)
            //    throw new InvalidQueryException("You must specify at least one place name in your query.");

            // Call the Web service and get the results.
            var manager = new SubscriptionManager(inputs.SubscriptionId);
            List<StorageAccount> accounts;
            try
            {
                var storageManager = manager.GetStorageManager();
                accounts = storageManager.ForStorageInformationQuery()
                    .AddCertificateFromStore(inputs.ManagementCertificateThumbprint)
                    .GetStorageAccountList(true);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Unable to query Windows Azure", ex);
            }

            return (IQueryable<T>)accounts.AsQueryable();
        }

        /// <summary>
        /// Returns a valid type based on what should be executed in this context
        /// </summary>
        public Type GetValidType()
        {
            return typeof (StorageAccount);
        }

        #endregion
    }
}
