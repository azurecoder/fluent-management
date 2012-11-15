using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Elastacloud.AzureManagement.Fluent.Types;

namespace Elastacloud.AzureManagement.Fluent.Services
{
    public interface IQueryCloudService
    {
        /// <summary>
        /// Used to list of the hosted services in a subscription
        /// </summary>
        /// <returns>A List<HostedServices> containing all of the cloud services in a subscription</HostedServices></returns>
        List<CloudService> GetHostedServiceList();

        /// <summary>
        /// Gets a list of cloud services with their attached deployments
        /// </summary>
        List<CloudService> GetCloudServiceListWithDeployments();
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
        List<CloudService> GetHostedServiceListContainingProductionDeployments(); 
    }
}
