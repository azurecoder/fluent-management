/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System.Security.Cryptography.X509Certificates;
using Elastacloud.AzureManagement.Fluent.Clients.Interfaces;
using Elastacloud.AzureManagement.Fluent.Commands.Services;
using Elastacloud.AzureManagement.Fluent.Helpers;
using Elastacloud.AzureManagement.Fluent.Types.Exceptions;

namespace Elastacloud.AzureManagement.Fluent.Clients
{
    /// <summary>
    /// Defines an interface which can perform operations on blobs
    /// </summary>
    public class ServiceBusClient : IServiceBusClient
    {
        private readonly string _subscriptionId;
        private readonly X509Certificate2 _managementCertificate;

        public ServiceBusClient(string subscriptionId, X509Certificate2 certificate)
        {
            _subscriptionId = subscriptionId;
            _managementCertificate = certificate;
        }

        #region Implementation of IServiceBusClient

        /// <summary>
        /// Creates a namespace given a name value 
        /// </summary>
        /// <param name="name">The name of the namespace that has to follow the prescribed rules</param>
        /// <param name="location">The location of the namespace default to N. Europe</param>
        public void CreateNamespace(string name, string location = LocationConstants.NorthEurope)
        {
            var validator = new ServiceBusNameValidator(name);
            if (!validator.ValidateName())
            {
                throw new FluentManagementException("Service bus namespace does not follow prescribed naming rules", "ServiceBusClient");
            }

            var command = new CreateServiceBusNamespaceCommand(name, location)
                {
                    SubscriptionId = _subscriptionId,
                    Certificate = _managementCertificate
                };
            command.Execute();
            Namespace = name;
        }

        /// <summary>
        /// The name of the service bus namespace
        /// </summary>
        public string Namespace { get; set; }

        #endregion
    }
}