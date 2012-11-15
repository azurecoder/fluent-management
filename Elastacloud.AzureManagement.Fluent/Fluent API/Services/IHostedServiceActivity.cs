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
        /// Deletes an existing hosted service and any deployments associated with it 
        /// </summary>
        /// <param name="name">the name of the hosted service</param>
        void DeleteExistingHostedService(string name);
    }
}