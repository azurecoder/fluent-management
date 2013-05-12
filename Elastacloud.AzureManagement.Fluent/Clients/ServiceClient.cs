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
using Elastacloud.AzureManagement.Fluent.Clients.Interfaces;
using Elastacloud.AzureManagement.Fluent.Commands.Certificates;
using Elastacloud.AzureManagement.Fluent.Commands.Services;
using Elastacloud.AzureManagement.Fluent.Helpers.PublishSettings;
using Elastacloud.AzureManagement.Fluent.Services.Classes;
using Elastacloud.AzureManagement.Fluent.Types;

namespace Elastacloud.AzureManagement.Fluent.Clients
{
    public class ServiceClient : IServiceClient 
    {
        /// <summary>
        /// Used to construct the ServiceClient
        /// </summary>
        public ServiceClient(string subscriptionId, X509Certificate2 certificate, string cloudService)
        {
            SubscriptionId = subscriptionId;
            ManagementCertificate = certificate;
            Name = cloudService;
        }

        /// <summary>
        /// The subscription being used
        /// </summary>
        public string SubscriptionId { get; set; }
        /// <summary>
        /// The management certificate previously uploaded to the portal
        /// </summary>
        public X509Certificate2 ManagementCertificate { get; set; }
        /// <summary>
        /// The name of the cloud service
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Adds a services certificate to the cloud service
        /// </summary>
        /// <param name="thumbprint">The thumbprint of the certificate to get from the store</param>
        /// <param name="password">The password used to export the key</param>
        public void UploadServiceCertificate(string thumbprint, string password)
        {
            X509Certificate2 certificate = null;
            try
            {
                certificate = PublishSettingsExtractor.FromStore(thumbprint, StoreLocation.LocalMachine);
            }
            catch (Exception)
            {
                certificate = certificate ?? PublishSettingsExtractor.FromStore(thumbprint);
                if(certificate == null)
                    throw new ApplicationException("unable to find certificate with thumbprint " + thumbprint + " in local store");
            }
            UploadServiceCertificate(certificate, password);            
        }
        /// <summary>
        /// Uploads a certificate given a valid service certificate and password
        /// </summary>
        /// <param name="certificate">The certificate being uploaded</param>
        /// <param name="password">The .pfx password for the certificate</param>
        public void UploadServiceCertificate(X509Certificate2 certificate, string password)
        {
            var certBytes = certificate.Export(X509ContentType.Pkcs12, password);
            var cert = new AddServiceCertificateCommand(certBytes, password, Name)
            {
                SubscriptionId = SubscriptionId,
                Certificate = ManagementCertificate
            };
            cert.Execute();
        }
        /// <summary>
        /// Creates a new cloud service 
        /// </summary>
        /// <param name="location">the data centre location of the cloud service</param>
        /// <param name="description">The description of the cloud service</param>
        public void CreateNewCloudService(string location, string description = "Fluent Management created cloud service")
        {
            var hostedServiceCreate = new CreateCloudServiceCommand(Name, description, location)
            {
                Certificate = ManagementCertificate,
                SubscriptionId = SubscriptionId
            };
            hostedServiceCreate.Execute();
        }

        /// <summary>
        /// Used to delete the current cloud service
        /// </summary>
        public void DeleteCloudService()
        {
            // delete the hosted service
            var deleteService = new DeleteHostedServiceCommand(Name)
            {
                Certificate = ManagementCertificate,
                SubscriptionId = SubscriptionId
            };
            deleteService.Execute();
        }

        /// <summary>
        /// Used to delete a deployment in a respective slot 
        /// </summary>
        /// <param name="slot">Either production or staging</param>
        public void DeleteDeployment(DeploymentSlot slot = DeploymentSlot.Production)
        {
            var deleteDeployment = new DeleteDeploymentCommand(Name, slot)
            {
                Certificate = ManagementCertificate,
                SubscriptionId = SubscriptionId
            };
            deleteDeployment.Execute();
        }

        public ServiceCertificate CreateServiceCertificateAndAddRemoteDesktop(string username, string password, ref CscfgFile file)
        {
            var certificate = new ServiceCertificate(username, password);
            certificate.Create();

            var desktop = new RemoteDesktop(certificate)
                              {
                                  Username = username,
                                  Password = password
                              };
            file.NewVersion = ((ICloudConfig) desktop).ChangeConfig(file.NewVersion);
            return certificate;
        }
    }
}
