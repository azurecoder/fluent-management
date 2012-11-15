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
using Elastacloud.AzureManagement.Fluent.Commands.Certificates;
using Elastacloud.AzureManagement.Fluent.Commands.Services;
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
        /// Used to construct the HostedServiceActivity
        /// </summary>
        /// <param name="manager">The DeploymentManager class</param>
        public HostedServiceActivity(DeploymentManager manager)
        {
            _manager = manager;
        }

        /// <summary>
        /// Used to create a hosted service with a service certificate if specified
        /// </summary>
        public void Create()
        {
            // create the hosted service here
            var service = new CreateHostedServiceCommand(_manager.HostedServiceName, _manager.Description, _manager.Location)
                              {
                                  Certificate = _manager.ManagementCertificate,
                                  SubscriptionId = _manager.SubscriptionId
                              };
            service.Execute();
            if (_manager.ServiceCertificate == null)
                return;
            // first of all upload the service certificate 
            byte[] export = _manager.ServiceCertificate.Certificate.Export(X509ContentType.Pkcs12, _manager.ServiceCertificate.PvkPassword);
            var command = new AddServiceCertificateCommand(export, _manager.ServiceCertificate.PvkPassword, _manager.HostedServiceName)
                              {
                                  Certificate = _manager.ManagementCertificate,
                                  SubscriptionId = _manager.SubscriptionId
                              };
            command.Execute();
        }

        public void Delete()
        {
            try
            {
                var deleteDeployment = new DeleteDeploymentCommand(_manager.HostedServiceName, _manager.DeploymentSlot)
                                           {
                                               Certificate = _manager.ManagementCertificate,
                                               SubscriptionId = _manager.SubscriptionId
                                           };
                deleteDeployment.Execute();
            }
            catch (Exception)
            {
                // no deployment here who cares!
            }
            // delete the hosted service
            var deleteService = new DeleteHostedServiceCommand(_manager.HostedServiceName)
                                    {
                                        Certificate = _manager.ManagementCertificate,
                                        SubscriptionId = _manager.SubscriptionId
                                    };
            deleteService.Execute();
        }
    }
}