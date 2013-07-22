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
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Elastacloud.AzureManagement.Fluent.Commands.VirtualMachines;
using Elastacloud.AzureManagement.Fluent.Helpers.PublishSettings;
using Elastacloud.AzureManagement.Fluent.Linq;
using Elastacloud.AzureManagement.Fluent.Types;
using Elastacloud.AzureManagement.Fluent.Types.VirtualMachines;
using Elastacloud.AzureManagement.Fluent.VirtualMachines.Classes;

namespace Elastacloud.AzureManagement.Fluent.LinqToAzure
{
    /// <summary>
    /// Used to execute a lambda expression against storage services
    /// </summary>
    public class VirtualMachineExecutor : IExecuteQuery
    {
        private IEnumerable<PersistentVMRole> _roles;
        /// <summary>
        /// The preparsed lambda expression for use with it's body
        /// </summary>
        private readonly LambdaExpression _expression;

        /// <summary>
        /// Constructs a StorageExecutor object
        /// </summary>
        /// <param name="expression">Used to construct the lambda expression for storage</param>
        public VirtualMachineExecutor(LambdaExpression expression)
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
            var sf = new VirtualMachineFinder(_expression != null ? _expression.Body : null);
            string cloudServiceName = sf.CloudServiceName;
            if(String.IsNullOrEmpty(cloudServiceName))
                throw new InvalidQueryException("You must specify a single cloud service in your query");

            // Call the Web service and get the results.
            if (_roles == null)
            {
                // get the cert in the form of an x509v3
                var certificate = PublishSettingsExtractor.FromStore(inputs.ManagementCertificateThumbprint);
                try
                {
                    var properties = new WindowsVirtualMachineProperties()
                        {
                            CloudServiceName = cloudServiceName
                        };
                    var command = new GetVirtualMachineContextCommand(properties)
                        {
                            SubscriptionId = inputs.SubscriptionId,
                            Certificate = certificate
                        };
                    command.Execute();
                    _roles = command.PersistentVm;
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("Unable to query Windows Azure", ex);
                }
            }
            //return _roles.AsQueryable();
           return (IQueryable<T>)_roles.AsQueryable();
        }

        /// <summary>
        /// Returns a valid type based on what should be executed in this context
        /// </summary>
        public Type GetValidType()
        {
            return typeof(IEnumerable<PersistentVMRole>);
        }

        #endregion
    }
}
