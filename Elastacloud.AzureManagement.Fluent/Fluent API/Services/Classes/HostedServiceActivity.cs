/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System;
using System.Security.Cryptography.X509Certificates;
using Elastacloud.AzureManagement.Fluent.Clients;
using Elastacloud.AzureManagement.Fluent.Commands.Certificates;
using Elastacloud.AzureManagement.Fluent.Commands.Services;
using Elastacloud.AzureManagement.Fluent.Helpers;
using Elastacloud.AzureManagement.Fluent.Types;

namespace Elastacloud.AzureManagement.Fluent.Services.Classes
{
    /// <summary>
    /// Used to create or delete a hosted service
    /// </summary>
    public class HostedServiceActivity
    {
        /// <summary>
        /// A private reference to the DeploymentManager containing the state for the operation
        /// </summary>
        private readonly DeploymentManager _manager;
        /// <summary>
        /// Service client for all cloud service requests
        /// </summary>
        private readonly ServiceClient _client;


        /// <summary>
        /// Used to construct the HostedServiceActivity
        /// </summary>
        /// <param name="manager">The DeploymentManager class</param>
        public HostedServiceActivity(DeploymentManager manager)
        {
            _manager = manager;
            _client = new ServiceClient(_manager.SubscriptionId, _manager.ManagementCertificate, _manager.HostedServiceName);
        }

        /// <summary>
        /// Used to create a hosted service with a service certificate if specified
        /// </summary>
        public void Create()
        {
            // create the hosted service here
            var description = _manager.Description ?? "Fluent Management created cloud service";
            var location = _manager.Location ?? LocationConstants.NorthEurope;
            _client.CreateNewCloudService(location, description);
            if (_manager.ServiceCertificate == null)
                return;
            // first of all upload the service certificate 
            _client.UploadServiceCertificate(_manager.ServiceCertificate.Certificate, _manager.ServiceCertificate.PvkPassword);
        }

        public void Delete()
        {
            try
            {
                _client.DeleteDeployment(_manager.DeploymentSlot);
            }
            catch
            {
                // no deployment here who cares!
            }
            _client.DeleteCloudService();
        }
    }
}