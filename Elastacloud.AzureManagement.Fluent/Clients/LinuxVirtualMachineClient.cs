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
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Elastacloud.AzureManagement.Fluent.Clients.Helpers;
using Elastacloud.AzureManagement.Fluent.Clients.Interfaces;
using Elastacloud.AzureManagement.Fluent.Commands.Blobs;
using Elastacloud.AzureManagement.Fluent.Commands.Services;
using Elastacloud.AzureManagement.Fluent.Commands.VirtualMachines;
using Elastacloud.AzureManagement.Fluent.Helpers;
using Elastacloud.AzureManagement.Fluent.Types.Exceptions;
using Elastacloud.AzureManagement.Fluent.Types.VirtualMachines;
using Elastacloud.AzureManagement.Fluent.VirtualMachines.Classes;

namespace Elastacloud.AzureManagement.Fluent.Clients
{
    public class LinuxVirtualMachineClient : ILinuxVirtualMachineClient
    {
        // The Vm role which is being used to hold the state of the linux virtual machine
        private List<PersistentVMRole> _vmRoles;

        /// <summary>
        /// Constructs a LinuxVirtualMachineClient and will get the details of a virtual machine given a cloud service
        /// </summary>
        /// <param name="subscriptionId">the subscription id </param>
        /// <param name="certificate">A management certificate for the subscription</param>
        public LinuxVirtualMachineClient(string subscriptionId, X509Certificate2 certificate)
        {
            SubscriptionId = subscriptionId;
            ManagementCertificate = certificate;
        }

        /// <summary>
        /// Constructs a VirtualMachinenClient
        /// </summary>
        /// <param name="properties">A valid VirtualMachineProperties object</param>
        /// <param name="subscriptionId">The susbcription id for the subscription</param>
        /// <param name="certificate">The susbcription id for the subscription</param>
        public LinuxVirtualMachineClient(List<LinuxVirtualMachineProperties> properties, string subscriptionId, X509Certificate2 certificate)
            : this(subscriptionId, certificate)
        {
            Properties = properties;
        }
        /// <summary>
        /// The virtual machine properties necessary to get any of the details for the virtual machine
        /// </summary>
        public List<LinuxVirtualMachineProperties> Properties { get; set; }

        /// <summary>
        /// Gets thye configuration for the virtual machine
        /// </summary>
        public List<PersistentVMRole> VirtualMachine { get; private set; }

        /// <summary>
        /// Creates a new virtual machine from a gallery template
        /// </summary>
        /// <param name="properties">Can be any gallery template</param>
        /// <param name="cloudServiceName">The name of the cloud service - if it doesn't exist it will be created</param>
        /// <param name="serviceCertificate">The service certificate responsible for adding the ssh keys</param>
        /// <param name="location">Where the cloud service will be created if it doesn't exist</param>
        public IVirtualMachineClient CreateNewVirtualMachineDeploymentFromTemplateGallery(List<LinuxVirtualMachineProperties> properties, string cloudServiceName, ServiceCertificateModel serviceCertificate = null, string location = LocationConstants.NorthEurope)
        {
            if(String.IsNullOrEmpty(cloudServiceName))
                throw new FluentManagementException("Cloud service name cannot be empty", "LinuxVirtualMachineClient");
            
            // if the cloud service doesn't exist we'll create it
            // first check that the service is available
            var checkCloudServiceAvailabilityCommand = new CheckCloudServiceNameAvailableCommand(cloudServiceName)
                {
                    SubscriptionId = SubscriptionId,
                    Certificate = ManagementCertificate
                };
            checkCloudServiceAvailabilityCommand.Execute();
            // when the check is complete we'll create the cloud service in the specified or default region if it doesn't exist
            if (checkCloudServiceAvailabilityCommand.CloudServiceAvailable)
            {
                var cloudServiceCommand = new CreateCloudServiceCommand(cloudServiceName, "Created by Fluent Management", location)
                    {
                        SubscriptionId = SubscriptionId,
                        Certificate = ManagementCertificate
                    };
                cloudServiceCommand.Execute();
            }
            // upload the service certificate if it exists for the ssh keys
            if (serviceCertificate != null)
            {
                var client = new ServiceClient(SubscriptionId, ManagementCertificate, cloudServiceName);
                client.UploadServiceCertificate(serviceCertificate.ServiceCertificate, serviceCertificate.Password);
            }

            // This is really unfortunate and not documented anywhere - unable to add multiple roles to a rolelist!!!
            // continue to the create the virtual machine in the cloud service
            var command = new CreateLinuxVirtualMachineDeploymentCommand(new List<LinuxVirtualMachineProperties>(new[]{properties[0]}), cloudServiceName)
            {
                SubscriptionId = SubscriptionId,
                Certificate = ManagementCertificate
            };
            command.Execute();
            // try and add the other concurrently
            // concurrency doesn't work - there is no way to build a fast deployment :-(
            for (int i = 1; i < properties.Count; i++)
            {
                // add this we'll need to ensure that it's populated for the command
                var theProperty = properties[i];
                theProperty.CloudServiceName = cloudServiceName;
                // clousrure may cause an exception here - proof is in the pudding!
                var startCommand = new AddLinuxVirtualMachineToDeploymentCommand(theProperty, cloudServiceName)
                    {
                        SubscriptionId = SubscriptionId,
                        Certificate = ManagementCertificate
                    };
                startCommand.Execute();
            }

            // important here to force a refresh - just in case someone to conduct an operation on the VM in a single step
            Properties = properties;
            // create a new client and return this so that properties can be populated automatically
            return new LinuxVirtualMachineClient(properties, SubscriptionId, ManagementCertificate);
        }

        /// <summary>
        /// the management certificate used to connect to the virtual machine
        /// </summary>
        protected X509Certificate2 ManagementCertificate { get; set; }
        /// <summary>
        /// the subscription id of the accoun which the virtual machine is present in 
        /// </summary>
        protected string SubscriptionId { get; set; }

      
        #region Implementation of IVirtualMachineClient

        /// <summary>
        /// Deletes the virtual machine that has context with the client
        /// </summary>
        /// <param name="removeDisks">True if the underlying disks in blob storage should be removed</param>
        /// <param name="removeCloudService">Removes the cloud service container</param>
        /// <param name="removeStorageAccount">The storage account that the vhd is in</param>
        public void DeleteVirtualMachine(bool removeDisks, bool removeCloudService, bool removeStorageAccount)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes a vm disk if a name is known
        /// </summary>
        /// <param name="name">The name of the vm disk</param>
        public void DeleteNamedVirtualMachineDisk(string name)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Restarts the virtual machine instance
        /// </summary>
        public void Restart()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Stops the virtual machine instance
        /// </summary>
        public void Stop()
        {
            foreach (var linuxVirtualMachineProperties in Properties)
            {
                // start the role up -- this could take a while the previous two operations are fairly lightweight
                // and the provisioning doesn't occur until the role starts not when it is created
                var stopCommand = new StopVirtualMachineCommand(linuxVirtualMachineProperties)
                {
                    SubscriptionId = SubscriptionId,
                    Certificate = ManagementCertificate
                };
                stopCommand.Execute();
            }
           
        }

        /// <summary>
        /// Gets the container that the storage blob resides in 
        /// </summary>
        public string StorageContainerName { get; private set; }

        /// <summary>
        /// The name of the blob which is stored for the vm
        /// </summary>
        public string StorageFileName { get; private set; }

        #endregion
    }
}
