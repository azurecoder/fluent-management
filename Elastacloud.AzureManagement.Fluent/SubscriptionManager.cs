/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System.Security.Cryptography.X509Certificates;
using Elastacloud.AzureManagement.Fluent.Helpers;
using Elastacloud.AzureManagement.Fluent.Helpers.PublishSettings;
using Elastacloud.AzureManagement.Fluent.Roles;
using Elastacloud.AzureManagement.Fluent.Services;
using Elastacloud.AzureManagement.Fluent.SqlAzure;
using Elastacloud.AzureManagement.Fluent.Storage;
using Elastacloud.AzureManagement.Fluent.Subscriptions;
using Elastacloud.AzureManagement.Fluent.Types;
using Elastacloud.AzureManagement.Fluent.VirtualMachines;
using Elastacloud.AzureManagement.Fluent.Watchers;

namespace Elastacloud.AzureManagement.Fluent
{
    /// <summary>
    /// Includes methods to reutrns set of Manager classes to manipulate things to do with storage, services, etc.
    /// </summary>
    public class SubscriptionManager
    {
        /// <summary>
        /// The subscription id used for all of the operations
        /// </summary>
        private readonly string _subscriptionId;

        /// <summary>, 
        /// Constructs a subscription manager which can be used to get other managers specific to the operations being requested
        /// </summary>
        /// <param name="subscriptionId">the subscription id</param>
        public SubscriptionManager(string subscriptionId, string defaultLocation = LocationConstants.NorthEurope)
        {
            _subscriptionId = subscriptionId;
            Location = defaultLocation;
        }

        public string Location { get; set; }

        /// <summary>
        /// Gets a manager to use on all types of deployment for PaaS
        /// </summary>
        public Services.ICertificateActivity GetDeploymentManager()
        {
            return new DeploymentManager(_subscriptionId, Location);
        }

        /// <summary>
        /// Gets a manager to use on Sql Azure 
        /// </summary>
        public SqlAzureManager GetSqlAzureManager()
        {
            return new SqlAzureManager(_subscriptionId);
        }

        /// <summary>
        /// Allows access to the storage manager which performs operations on storage
        /// </summary>
        /// <returns>A StorageManager instance</returns>
        public StorageManager GetStorageManager()
        {
            return new StorageManager(_subscriptionId);
        }

        /// <summary>
        /// Allows access to a SubscriptionDetailsManager so that information about the subscription such as locations allowed etc. cna be returned
        /// </summary>
        /// <returns>A SubscriptionDetailsManager instance</returns>
        public Subscriptions.ICertificateActivity GetSubscriptionDetailsManager()
        {
            return new SubscriptionDetailsManager(_subscriptionId, Location);
        }

        /// <summary>
        /// Gets a manager to perform operations on virtual machines
        /// </summary>
        /// <returns>An VirtualMachineManager instance</returns>
        public VirtualMachines.ICertificateActivity GetVirtualMachinesManager()
        {
            return new VirtualMachineManager(_subscriptionId);
        }

        /// <summary>
        /// Returns a watcher which will indicate when the status of the deployment and role by extension has changed
        /// </summary>
        /// <param name="serviceName">the name of the hosted service</param>
        /// <param name="roleName">the name of the role</param>
        /// <param name="slot">the name of the deployment slot either production or staging</param>
        /// <param name="certificateThumbprint">the thumbprint of the management certificate</param>
        /// <returns>A DeploymentStatusWatcher class instance</returns>
        public DeploymentStatusWatcher GetRoleStatusChangedWatcher(string serviceName, string roleName, DeploymentSlot slot,
                                                             string certificateThumbprint)
        {
            X509Certificate2 certificate = PublishSettingsExtractor.FromStore(certificateThumbprint);
            return new DeploymentStatusWatcher(serviceName, slot, certificate, _subscriptionId);
        }

        /// <summary>
        /// Added the deployment 
        /// </summary>
        /// <param name="certificateThumbprint">The thumbprint of the certificate being used to update the deployment status</param>
        /// <param name="serviceName">The name of the cloud service</param>
        /// <param name="slot">The deployment slot</param>
        /// <returns>A RoleContextManager class instance</returns>
        public RoleContextManager GetRoleContextManager(string certificateThumbprint, string serviceName, DeploymentSlot slot)
        {
            X509Certificate2 certificate = PublishSettingsExtractor.FromStore(certificateThumbprint);
            return new RoleContextManager(_subscriptionId, certificate, serviceName, slot);
        }

    }
}