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
using System.Security.Cryptography.X509Certificates;
using Elastacloud.AzureManagement.Fluent.Clients.Interfaces;
using Elastacloud.AzureManagement.Fluent.Commands.Certificates;
using Elastacloud.AzureManagement.Fluent.Commands.Services;
using Elastacloud.AzureManagement.Fluent.Commands.Subscriptions;
using Elastacloud.AzureManagement.Fluent.Helpers;
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
        public ServiceClient(string subscriptionId, X509Certificate2 certificate, string cloudService, DeploymentSlot slot = DeploymentSlot.Production)
        {
            SubscriptionId = subscriptionId;
            ManagementCertificate = certificate;
            Name = cloudService;
            Slot = slot;
        }

        /// <summary>
        /// gets or sets the deployment slot for the cloud service
        /// </summary>
        public DeploymentSlot Slot { get; set; }

        /// <summary>
        /// Starts all of the roles within a cloud service
        /// </summary>
        public void Start()
        {
            var command = new UpdateRoleStatusCommand(Name, Slot, UpdateDeploymentStatus.Running)
                {
                    SubscriptionId = SubscriptionId,
                    Certificate = ManagementCertificate
                };
            command.Execute();
        }

        /// <summary>
        /// Stops all of the roles within a cloud service
        /// </summary>
        public void Stop()
        {
            var command = new UpdateRoleStatusCommand(Name, Slot, UpdateDeploymentStatus.Suspended)
            {
                SubscriptionId = SubscriptionId,
                Certificate = ManagementCertificate
            };
            command.Execute();
        }

        public bool IsCloudServiceNameAvailable
        {
            get
            {
                var command = new CheckCloudServiceNameAvailableCommand(Name)
                {
                    SubscriptionId = SubscriptionId,
                    Certificate = ManagementCertificate
                };
                command.Execute();

                return command.CloudServiceAvailable;
            }
        }

        /// <summary>
        /// Gets a list of available locations for the subscription
        /// </summary>
        public List<LocationInformation> AvailableLocations
        {
            get
            {
                var command = new GetSubscriberLocationsCommand
                {
                    SubscriptionId = SubscriptionId,
                    Certificate = ManagementCertificate
                };
                command.Execute();
                return command.Locations;
            }
        }

        /// <summary>
        /// This is the account that will be used for any of the storage interactions 
        /// </summary>
        public string DefaultStorageAccount { get; set; }

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
        /// Gets the deployment name of the current deployment
        /// </summary>
        public string DeploymentName
        {
            get
            {
                var command = new GetCloudServicePropertiesCommand(Name)
                    {
                        SubscriptionId = SubscriptionId,
                        Certificate = ManagementCertificate
                    };
                command.Execute();
                return command.CloudServiceDeployments[0].Name;
            }
        }
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
        /// <param name="includePrivateKey">The .pfx password for the certificate</param>
        public void UploadServiceCertificate(X509Certificate2 certificate, string password = "", bool includePrivateKey = false)
        {
            var certBytes = includePrivateKey
                                ? certificate.Export(X509ContentType.Pkcs12, password)
                                : certificate.Export(X509ContentType.Cert);
            
            var cert = new AddServiceCertificateCommand(certBytes, password, Name)
            {
                SubscriptionId = SubscriptionId,
                Certificate = ManagementCertificate
            };
            cert.Execute();
        }

        /// <summary>
        /// Creates a service certificate
        /// </summary>
        /// <param name="name">The name (CN) of the certificate</param>
        /// <param name="password">The password of the certificate</param>
        /// <param name="exportDirectory">Where the .pem, .cer and pfx will be put</param>
        public CertificateGenerator CreateServiceCertificateExportToFileSystem(string name, string password, string exportDirectory)
        {
            var generator = BuildCertGenerator(name, password); 
            generator.ExportToFileSystem(exportDirectory);
            return generator;
        }

        /// <summary>
        /// Exports a service certificate to Windows Azure Storage
        /// </summary>
        public CertificateGenerator CreateServiceCertificateExportToStorage(string name, string password, string storageAccountName,
            string container, string folder)
        {
            var generator = BuildCertGenerator(name, password);
            CertificateBlobLocation = generator.ExportToStorageAccount(storageAccountName, container, folder);
            return generator;
        }

        /// <summary>
        /// Gets a list of cloud services in the currently subscription
        /// </summary>
        public List<string> CloudServicesInSubscription
        {
            get
            {
                var command = new GetHostedServiceListCommand()
                {
                    Certificate = ManagementCertificate,
                    SubscriptionId = SubscriptionId
                };
                command.Execute();
                return
                    command.HostedServices.OrderByDescending(service => service.Name)
                        .Select(service => service.Name)
                        .ToList();
            }
        }

        /// <summary>
        /// Deletes a cloud service and removes all of the .vhds for vms associated with the production deployment
        /// </summary>
        /// <param name="deploymentName">The name of the deployment to delete</param>
        public void DeleteCloudServiceAndDeployment(string deploymentName)
        {
            var command = new DeleteCloudServiceAndDeploymentCommand(Name, deploymentName)
            {
                SubscriptionId = SubscriptionId,
                Certificate = ManagementCertificate
            };
            command.Execute();
            var csDelete = new DeleteHostedServiceCommand(Name)
            {
                SubscriptionId = SubscriptionId,
                Certificate = ManagementCertificate
            };
            csDelete.Execute();
        }

        /// <summary>
        /// Gets the root certificate blob location for the .pem, .pfx and .cer files
        /// </summary>
        public string CertificateBlobLocation { get; private set; }

        private CertificateGenerator BuildCertGenerator(string name, string password)
        {
            var generator = new CertificateGenerator(SubscriptionId, ManagementCertificate);
            generator.Create(name, DateTime.UtcNow.Subtract(TimeSpan.FromDays(1)), DateTime.UtcNow.AddYears(2), password);
            return generator;
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

        /// <summary>
        /// Updates a role instance count within a cloud services
        /// </summary>
        public void UpdateRoleInstanceCount(string roleName, int instanceCount)
        {
            var config = new GetDeploymenConfigurationCommand(Name)
                {
                    SubscriptionId = SubscriptionId,
                    Certificate = ManagementCertificate
                };
            config.Execute();
            config.Configuration.SetInstanceCountForRole(roleName, instanceCount);
            var update = new SetDeploymenConfigurationCommand(Name, config.Configuration)
                {
                    SubscriptionId = SubscriptionId,
                    Certificate = ManagementCertificate
                };
            update.Execute();
        }

        /// <summary>
        /// Creates a service certificate and adds to the remote config 
        /// </summary>
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

        /// <summary>
        /// Returns a list of roles for a given cloud service
        /// </summary>
        public List<string> Roles
        {
            get
            {
                var command = new GetDeploymenRoleNamesCommand(Name, Slot)
                    {
                        SubscriptionId = SubscriptionId,
                        Certificate = ManagementCertificate
                    };
                command.Execute();
                return command.RoleNames;
            }
        }
    }
}
