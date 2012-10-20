/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System.Collections.Generic;
using Elastacloud.AzureManagement.Fluent.Types;

namespace Elastacloud.AzureManagement.Fluent.Services
{
    /// <summary>
    /// A hosted service activity used to update, add or remove a hosted service
    /// </summary>
    public interface IHostedServiceActivity
    {
        /// <summary>
        /// Generally used to add a deployment to an existing hosted service
        /// </summary>
        /// <param name="name">The name of the hosted service</param>
        /// <returns>An IDeploymentConfigurationStorageActivity interface</returns>
        IDeploymentConfigurationStorageActivity WithExistingHostedService(string name);

        /// <summary>
        /// Adds a new hosted service to a subscription
        /// </summary>
        /// <param name="name">The name of the new hosted service</param>
        /// <returns>An IDeploymentConfigurationStorageActivity interface</returns>
        IDeploymentConfigurationStorageActivity WithNewHostedService(string name);

        /// <summary>
        /// Used to list of the hosted services in a subscription
        /// </summary>
        /// <returns>A List<HostedServices> containing all of the cloud services in a subscription</HostedServices></returns>
        List<HostedService> GetHostedServiceList();

        /// <summary>
        /// Gets a list of role names for a particular hosted service in the production slot
        /// </summary>
        /// <param name="serviceName">The name of the service</param>
        /// <returns>A string list</returns>
        List<string> GetRoleNamesForProductionDeploymentForServiceWithName(string serviceName);
        /// <summary>
        /// Gets the deployment configuration in the production slot for a cloud service
        /// </summary>
        /// <param name="serviceName">The name of the cloud service</param>
        /// <returns>A CscfgFile instance</returns>
        CscfgFile GetConfigurationForProductionDeploymentForServiceWithName(string serviceName);
        /// <summary>
        /// Gets a list of hosted services that contain production deployments for the subscription
        /// </summary>
        /// <returns>A list of hosted services</returns>
        List<HostedService> GetHostedServiceListContainingProductionDeployments(); 

        /// <summary>
        /// Deletes an existing hosted service and any deployments associated with it 
        /// </summary>
        /// <param name="name">the name of the hosted service</param>
        void DeleteExistingHostedService(string name);
    }
}