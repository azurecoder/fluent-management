﻿/*****************************************************************`*******************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using Elastacloud.AzureManagement.Fluent.Commands.VirtualNetworks;
using Elastacloud.AzureManagement.Fluent.Helpers;
using Elastacloud.AzureManagement.Fluent.Types;
using Elastacloud.AzureManagement.Fluent.Types.Exceptions;
using Elastacloud.AzureManagement.Fluent.Types.VirtualMachines;
using Elastacloud.AzureManagement.Fluent.Types.VirtualNetworks;
using Elastacloud.AzureManagement.Fluent.VirtualMachines.Classes;

namespace Elastacloud.AzureManagement.Fluent.Clients
{
    public class LinuxVirtualMachineClient : ILinuxVirtualMachineClient
    {
        // The Vm role which is being used to hold the state of the linux virtual machine
        private List<PersistentVMRole> _vmRoles;

        public event EventHandler<LinuxVirtualMachineProperties> LinuxVirtualMachineCreationEvent;
        public event EventHandler<VirtualMachineStatus> LinuxVirtualMachineStatusEvent;
       
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
        /// The service certificate that is deployed with the Linux VM
        /// </summary>
        public byte[] ServiceCertificate { get; private set; }

        /// <summary>
        /// The virtual machine properties necessary to get any of the details for the virtual machine
        /// </summary>
        public List<LinuxVirtualMachineProperties> Properties { get; set; }

        /// <summary>
        /// Gets thye configuration for the virtual machine
        /// </summary>
        // TODO: PULL BACK THE CORRECT VALUE FROM HERE
        public List<PersistentVMRole> VirtualMachine
        {
            get
            {
                if (_vmRoles != null)
                    return _vmRoles;
                var command = new GetVirtualMachineContextCommand(Properties.First())
                    {
                        SubscriptionId = SubscriptionId,
                        Certificate = ManagementCertificate
                    };
                command.Execute();
                return (_vmRoles = command.PersistentVm.ToList());
            }
        }

        private void CheckVmDeploymentIsRunning(List<LinuxVirtualMachineProperties> properties)
        {
            // 1. Get the number of vms in the role and create a binary list 
            var linuxProperties = new Dictionary<string, RoleInstanceStatus>();
            properties.ForEach(property => linuxProperties.Add(property.HostName, RoleInstanceStatus.RoleStateUnknown));
            var vmProperties = new LinuxVirtualMachineProperties() {CloudServiceName = properties[0].CloudServiceName};
            // 2. Set the value to if the vm is running or not 
            int index = 0;
            // time this out just in case after an hour?
            while (linuxProperties.Any(role => role.Value != RoleInstanceStatus.ReadyRole) || index == 360)
            {
                var command = new GetVirtualMachineContextCommand(vmProperties)
                {
                    SubscriptionId = SubscriptionId,
                    Certificate = ManagementCertificate
                };
                command.Execute();
                command.PersistentVm.ForEach(vm =>
                {
                    if (linuxProperties[vm.RoleName] == vm.Status) return;
                    // raise the event with the old and new status
                    LinuxVirtualMachineStatusEvent(this, new VirtualMachineStatus()
                    {
                        NewStatus = vm.Status,
                        OldStatus = linuxProperties[vm.RoleName],
                        VirtualMachineInstanceName = vm.RoleName,
                        CloudService = vmProperties.CloudServiceName
                    });
                    // update the status in the dictionary
                    linuxProperties[vm.RoleName] = vm.Status;
                });
                index++;
                // TODO: advice from Zak on the best way to use task.delay instead
                Thread.Sleep(TimeSpan.FromSeconds(10));
            }

            if (index == 360)
            {
                throw new FluentManagementException("timed out listening for status changes - check vms are running correctly", "LinuxVirtualMachineClient");       
            }
        }

        /// <summary>
        /// Creates a new virtual machine from a gallery template
        /// </summary>
        /// <param name="properties">Can be any gallery template</param>
        /// <param name="cloudServiceName">The name of the cloud service - if it doesn't exist it will be created</param>
        /// <param name="serviceCertificate">The service certificate responsible for adding the ssh keys</param>
        /// <param name="location">Where the cloud service will be created if it doesn't exist</param>
        /// <param name="affinityGroup">Affinity group that this service will live in</param>
        public IVirtualMachineClient CreateNewVirtualMachineDeploymentFromTemplateGallery(List<LinuxVirtualMachineProperties> properties, string cloudServiceName, ServiceCertificateModel serviceCertificate = null, string location = LocationConstants.NorthEurope, string affinityGroup = "")
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
            Trace.WriteLine(String.Format("Checked cloud service availability - is available: {0}", checkCloudServiceAvailabilityCommand.CloudServiceAvailable));
            // when the check is complete we'll create the cloud service in the specified or default region if it doesn't exist
            if (checkCloudServiceAvailabilityCommand.CloudServiceAvailable)
            {
                var cloudServiceCommand = new CreateCloudServiceCommand(cloudServiceName, "Created by Fluent Management", location, affinityGroup)
                    {
                        SubscriptionId = SubscriptionId,
                        Certificate = ManagementCertificate
                    };
                cloudServiceCommand.Execute();
                Trace.WriteLine(String.Format("Cloud service named {0} has been created", cloudServiceName));
            }
            // adds service certificate to the deployment
            if (serviceCertificate != null)
            {
                AddServiceCertificateToRoles(serviceCertificate, cloudServiceName, ref properties);
                Trace.WriteLine("A new service certificate has been added to the cloud service");
                ServiceCertificate = serviceCertificate.ServiceCertificate.Export(X509ContentType.Pfx);
            }
           
            // This is really unfortunate and not documented anywhere - unable to add multiple roles to a rolelist!!!
            // continue to the create the virtual machine in the cloud service
            var command = new CreateLinuxVirtualMachineDeploymentCommand(new List<LinuxVirtualMachineProperties>(new[]{properties[0]}), cloudServiceName)
            {
                SubscriptionId = SubscriptionId,
                Certificate = ManagementCertificate
            };
            command.Execute();
            // raise this event every time we create a VM
            if (LinuxVirtualMachineCreationEvent != null)
            {
                LinuxVirtualMachineCreationEvent(this, properties[0]);
            }
            Trace.WriteLine("Deployment created and first virtual machine added");
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
                // raise this event every time we create a VM
                if (LinuxVirtualMachineCreationEvent != null)
                {
                    LinuxVirtualMachineCreationEvent(this, theProperty);
                }
                Trace.WriteLine(String.Format("New VM added to deployment with hostname {0}", theProperty.HostName));
            }
            if (LinuxVirtualMachineStatusEvent != null)
            {
                Task.Factory.StartNew(() => CheckVmDeploymentIsRunning(properties));
            }

            // important here to force a refresh - just in case someone to conduct an operation on the VM in a single step
            Properties = properties;
            // create a new client and return this so that properties can be populated automatically
            return new LinuxVirtualMachineClient(properties, SubscriptionId, ManagementCertificate);
        }


        /// <summary>
        /// Lists all of the OS images assoiated with Linux that are public within the subscription
        /// </summary>
        public List<ImageProperties> ListImages(string filter = "")
        {
            var command = new ListImagesCommand()
            {
                SubscriptionId = SubscriptionId,
                Certificate = ManagementCertificate
            };
            command.Execute();

            // groups the response back from the image list
            // and takes the most recent publication of the image from the group
            var list = command.Properties.Where(image => image.OperatingSystem == PlatformType.Linux).ToList();
            if (!String.IsNullOrEmpty(filter))
            {
                var newImageList = new List<ImageProperties>();
                var imageList = list.Where(image => !String.IsNullOrEmpty(image.ImageFamily) && image.ImageFamily.Contains(filter))
                    .GroupBy(image => image.ImageFamily);
                foreach (var group in imageList)
                {
                    ImageProperties lImageProperties = null;
                    foreach (var image in group)
                    {
                        if (lImageProperties == null)
                            lImageProperties = image;
                        if (image.PublishedDate > lImageProperties.PublishedDate)
                            lImageProperties = image;
                    }
                    newImageList.Add(lImageProperties);
                }
                return newImageList;
            }
            return list;
        }

        /// <summary>
        /// Gets a list of virtual machine subnets that the vms in the cloud service belong to
        /// </summary>
        public CloudServiceNetworking GetVirtualMachineSubnetCollection(string cloudServiceName)
        {
            Properties = new List<LinuxVirtualMachineProperties>
            {
                new LinuxVirtualMachineProperties() {CloudServiceName = cloudServiceName}
            };
            string virtualNetwork = VirtualMachine[0].VirtualNetworkName;

            if (virtualNetwork == null)
            {
                return null;
            }

            var subnets = VirtualMachine.Select(vm => vm.NetworkConfigurationSet.SubnetName).Distinct().ToList();
            return new CloudServiceNetworking()
            {
                VirtualNetworkName = virtualNetwork,
                Subnets = subnets
            };
        }

        private void AddServiceCertificateToRoles(ServiceCertificateModel serviceCertificate, string cloudServiceName, ref List<LinuxVirtualMachineProperties> properties)
        {
            // upload the service certificate if it exists for the ssh keys
            if (serviceCertificate != null)
            {
                var client = new ServiceClient(SubscriptionId, ManagementCertificate, cloudServiceName);
                client.UploadServiceCertificate(serviceCertificate.ServiceCertificate, serviceCertificate.Password, true);
                foreach (var linuxVirtualMachineProperty in properties)
                {
                    linuxVirtualMachineProperty.PublicKeys.Add(new SSHKey(KeyType.PublicKey)
                    {
                        FingerPrint = serviceCertificate.ServiceCertificate.GetCertHashString(),
                        Path = String.Format("/home/{0}/.ssh/authorized_keys", linuxVirtualMachineProperty.UserName)
                    });
                    linuxVirtualMachineProperty.KeyPairs.Add(new SSHKey(KeyType.KeyPair)
                    {
                        FingerPrint = serviceCertificate.ServiceCertificate.GetCertHashString(),
                        Path = String.Format("/home/{0}/.ssh/id_rsa", linuxVirtualMachineProperty.UserName)
                    });
                    
                    linuxVirtualMachineProperty.DisableSSHPasswordAuthentication = true;
                }
            }
        }

        public void AddRolesToExistingDeployment(List<LinuxVirtualMachineProperties> properties, string cloudServiceName, ServiceCertificateModel serviceCertificate)
        {
            // add the service certificate
            AddServiceCertificateToRoles(serviceCertificate, cloudServiceName, ref properties);
            // concurrency doesn't work - there is no way to build a fast deployment :-(
            foreach (var theProperty in properties)
            {
                theProperty.CloudServiceName = cloudServiceName;
                // clousrure may cause an exception here - proof is in the pudding!
                var startCommand = new AddLinuxVirtualMachineToDeploymentCommand(theProperty, cloudServiceName)
                    {
                        SubscriptionId = SubscriptionId,
                        Certificate = ManagementCertificate
                    };
                startCommand.Execute();
                Trace.WriteLine(String.Format("Added VM with hostname {0} to deployment", theProperty.HostName));
            }
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
        /// <param name="removeUnderlyingBlobs">removes the underlying blob</param>
        /// <param name="removeCloudService">Removes the cloud service container</param>
        /// <param name="removeStorageAccount">The storage account that the vhd is in</param>
        public void DeleteVirtualMachine(bool removeDisks = true, bool removeUnderlyingBlobs = true, bool removeCloudService = true, bool removeStorageAccount = true)
        {
            //TODO: write a get method for the virtual machine properties
            IBlobClient blobClient = null;
            foreach (var vm in VirtualMachine)
            {
                // create the blob client
                string diskName = vm.OSHardDisk.DiskName;
                string storageAccount = ParseBlobDetails(vm.OSHardDisk.MediaLink);
                // create the blob client
                blobClient = new BlobClient(SubscriptionId, StorageContainerName, storageAccount, ManagementCertificate);

                // find the property
                var linuxVirtualMachineProperty = Properties.Find(a => a.RoleName == vm.RoleName);
                // if this property is null then we don't want it to impede the operation
                if (linuxVirtualMachineProperty == null)
                    continue;
                // first delete the virtual machine command
                var deleteVirtualMachine = new DeleteVirtualMachineCommand(linuxVirtualMachineProperty)
                    {
                        SubscriptionId = SubscriptionId,
                        Certificate = ManagementCertificate
                    };
                try
                {
                    deleteVirtualMachine.Execute();
                    Trace.WriteLine(String.Format("Deleted virtual machine {0}", linuxVirtualMachineProperty.HostName));
                }
                catch (Exception)
                {
                    if (VirtualMachine.Count == 1)
                    {
                        // should be a 400 here if this is the case then there is only a single role in the deployment - quicker to do it this way!
                        var deleteVirtualMachineDeployment =
                            new DeleteVirtualMachineDeploymentCommand(linuxVirtualMachineProperty)
                                {
                                    SubscriptionId = SubscriptionId,
                                    Certificate = ManagementCertificate
                                };
                        deleteVirtualMachineDeployment.Execute();
                    }
                }

                if (!removeDisks)
                    continue;

                // when this is finished we'll delete the operating system disk - check this as we may need to putin a pause
                // remove the disk association
                DeleteNamedVirtualMachineDisk(diskName);
                // remove the data disks
                DeleteDataDisks(vm, removeUnderlyingBlobs ? blobClient : null);
                if (removeUnderlyingBlobs)
                {
                    // remove the physical disk
                    blobClient.DeleteBlob(StorageFileName);
                }
            }
            // if we need to use the first cloud service
            // remove the cloud services
            if (removeCloudService)
            {
                // delete the cloud service here
                var deleteCloudService = new DeleteHostedServiceCommand(Properties[0].CloudServiceName)
                {
                    SubscriptionId = SubscriptionId,
                    Certificate = ManagementCertificate
                };
                deleteCloudService.Execute();
            }
            // remove the storage account
            if (removeStorageAccount)
            {
                blobClient.DeleteStorageAccount();
            }
        }

        /// <summary>
        /// Deletes a vm disk if a name is known
        /// </summary>
        /// <param name="name">The name of the vm disk</param>
        public void DeleteNamedVirtualMachineDisk(string name)
        {
            bool diskErased = false;
            int count = 0;
            // keep this going until we delete the disk or time out!
            while (count < 100 && !diskErased)
            {
                try
                {
                    var deleteVirtualMachineDisk = new DeleteVirtualMachineDiskCommand(name)
                    {
                        SubscriptionId = SubscriptionId,
                        Certificate = ManagementCertificate
                    };
                    deleteVirtualMachineDisk.Execute();
                    diskErased = true;
                }
                catch (Exception)
                {
                    count++;
                    Thread.Sleep(3000);
                }
            }
        }

        private void DeleteDataDisks(PersistentVMRole vm, IBlobClient client)
        {
            // delete the data disks in the reverse order
            if (vm.HardDisks.HardDiskCollection == null) return;
            for (int i = vm.HardDisks.HardDiskCollection.Count - 1; i >= 0; i--)
            {
                var dataDiskCommand = new DeleteVirtualMachineDiskCommand(vm.HardDisks.HardDiskCollection[i].DiskName)
                {
                    SubscriptionId = SubscriptionId,
                    Certificate = ManagementCertificate
                };
                dataDiskCommand.Execute();

                int pos = vm.HardDisks.HardDiskCollection[i].MediaLink.LastIndexOf('/');
                string diskFile = vm.HardDisks.HardDiskCollection[i].MediaLink.Substring(pos + 1);
                if (client != null)
                    client.DeleteBlob(diskFile);
            }
        }

        /// <summary>
        /// Restarts the virtual machine instance
        /// </summary>
        public void Restart()
        {
            // start the role up -- this could take a while the previous two operations are fairly lightweight
            // and the provisioning doesn't occur until the role starts not when it is created
            Trace.WriteLine(String.Format("Restarting {0} virtual machines in deployment {1} in cloud service {2}",
               Properties.Count, Properties.First().DeploymentName, Properties.First().CloudServiceName));
            foreach (var linuxVirtualMachineProperty in Properties)
            {
                var restartCommand = new StartVirtualMachineCommand(linuxVirtualMachineProperty)
                {
                    SubscriptionId = SubscriptionId,
                    Certificate = ManagementCertificate
                };
                restartCommand.Execute();
                Trace.WriteLine(String.Format("VM restarted with hostname {0}", linuxVirtualMachineProperty.HostName));
            }
            
        }

        /// <summary>
        /// Stops the virtual machine instance
        /// </summary>
        public void Stop()
        {
            Trace.WriteLine(String.Format("Stopping {0} virtual machines in deployment {1} in cloud service {2}", 
                Properties.Count, Properties.First().DeploymentName, Properties.First().CloudServiceName));
            foreach (LinuxVirtualMachineProperties linuxVirtualMachineProperties in Properties)
            {
                var stopCommand = new StopVirtualMachineCommand(linuxVirtualMachineProperties)
                    {
                        SubscriptionId = SubscriptionId, 
                        Certificate = ManagementCertificate
                    };
                stopCommand.Execute();
                Trace.WriteLine(String.Format("VM stopped and deprovisioned with hostname {0}", linuxVirtualMachineProperties.HostName));
            }
        }

        /// <summary>
        /// Returns the name of the blob and container 
        /// </summary>
        /// <param name="blobAddress"></param>
        private string ParseBlobDetails(string blobAddress)
        {
            var helper = new UrlHelper(blobAddress);
            StorageContainerName = helper.Path;
            StorageFileName = helper.File;
            return helper.HostSubDomain;
        }
        /// <summary>
        /// Gets the container that the storage blob resides in 
        /// </summary>
        public string StorageContainerName { get; private set; }

        /// <summary>
        /// The name of the blob which is stored for the vm
        /// </summary>
        public string StorageFileName { get; private set; }

        /// <summary>
        /// Cleans up any disks which don't have an attached VM
        /// </summary>
        public void CleanupUnattachedDisks()
        {
            // get all of the disks in the subscription
            var command = new GetVirtualDisksCommand()
                {
                    SubscriptionId = SubscriptionId,
                    Certificate = ManagementCertificate
                };
            command.Execute();
            var disks = command.Disks;
            // initiate the blob delete

            // iterate through the disks and clean up the unattached ones
            foreach (var disk in disks)
            {
                // make sure the disk is unattached
                if (disk.VM != null) continue;
                // get the blob details
                string storageAccount = ParseBlobDetails(disk.MediaLink);
                IBlobClient blobClient = new BlobClient(SubscriptionId, StorageContainerName, storageAccount, ManagementCertificate);
                DeleteNamedVirtualMachineDisk(disk.Name);
                Trace.WriteLine(String.Format("Deleting disk with name {0}", disk.Name));
                // delete the underlying blob
                blobClient.DeleteBlob(StorageFileName);
                Trace.WriteLine(String.Format("Deleting disk with name {0}", StorageFileName));
            }
        }

        /// <summary>
        /// Gets a list of hosts, internal ip addresses and other things
        /// </summary>
        public List<VmHost> GetAllInternalHostDetails()
        {
            var hosts = new List<VmHost>();
            VirtualMachine.ForEach(vm => hosts.Add(new VmHost()
            {
                HostName = vm.RoleName,
                IpAddress = vm.IPAddress,
                RoleName = vm.RoleName
            }));
            return hosts;
        }
        /// <summary>
        /// This returns all of the available ubuntu images in the gallery
        /// </summary>
        public ImageProperties GetCurrentUbuntuImage()
        {
            return ListImages("Ubuntu").Where(
                image => image.Label.Contains("Ubuntu Server 14.10") && !image.Label.Contains("DAILY"))
                .Take(1)
                .FirstOrDefault();
        }

        public List<VirtualNetworkSite> GetAvailableVirtualNetworks()
        {
            var command = new ListVirtualNetworksCommand()
            {
                SubscriptionId = SubscriptionId,
                Certificate = ManagementCertificate
            };
            command.Execute();
            return command.VirtualNetworks;
        }

        #endregion
    }


}
