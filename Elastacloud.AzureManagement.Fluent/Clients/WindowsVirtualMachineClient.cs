/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/
using System;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
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
    public class WindowsVirtualMachineClient : IVirtualMachineClient
    {
        // the name of the cloud service to look up 
        private string _cloudServiceName;
        // The Vm role which is being used to hold the state of the windows virtual machine
        private PersistentVMRole _vmRole;

        /// <summary>
        /// Constructs a WindowsVirtualMachineClient and will get the details of a virtual machine given a cloud service
        /// </summary>
        /// <param name="subscriptionId">the subscription id </param>
        /// <param name="certificate">A management certificate for the subscription</param>
        /// <param name="cloudServiceName">A cloud service which is in the subscription and contains the virtual machine</param>
        public WindowsVirtualMachineClient(string subscriptionId, X509Certificate2 certificate, string cloudServiceName = null)
        {
            SubscriptionId = subscriptionId;
            ManagementCertificate = certificate;
            // given a cloud service we want to be able to work out whether this contain a persistent vm role 
            _cloudServiceName = cloudServiceName;
        }

        /// <summary>
        /// Constructs a VirtualMachinenClient
        /// </summary>
        /// <param name="properties">A valid VirtualMachineProperties object</param>
        public WindowsVirtualMachineClient(WindowsVirtualMachineProperties properties)
        {
            Properties = properties;
        }
        /// <summary>
        /// The virtual machine properties necessary to get any of the details for the virtual machine
        /// </summary>
        public WindowsVirtualMachineProperties Properties { get; set; }

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
        /// Creates a new virtual machine from a gallery template
        /// </summary>
        /// <param name="properties">Can be any gallery template</param>
        public IVirtualMachineClient CreateNewVirtualMachineFromTemplateGallery(WindowsVirtualMachineProperties properties)
        {
            // for the time being we're going to adopt the default powershell cmdlet behaviour and always create a new cloud services
            EnsureVirtualMachineProperties(properties);
            if (!properties.UseExistingCloudService)
            {
                var cloudServiceCommand = new CreateCloudServiceCommand(properties.CloudServiceName,"Created by Fluent Management", properties.Location)
                                              {
                                                  SubscriptionId = properties.SubscriptionId,
                                                  Certificate = properties.Certificate
                                              };
                cloudServiceCommand.Execute();
            }
            // continue to the create the virtual machine in the cloud service
            var command = new CreateWindowsVirtualMachineDeploymentCommand(properties)
            {
                SubscriptionId = properties.SubscriptionId,
                Certificate = properties.Certificate
            };
            command.Execute();
            // start the role up -- this could take a while the previous two operations are fairly lightweight
            // and the provisioning doesn't occur until the role starts not when it is created
            var startCommand = new StartVirtualMachineCommand(properties)
            {
                SubscriptionId = properties.SubscriptionId,
                Certificate = properties.Certificate
            };
            startCommand.Execute();
            // important here to force a refresh - just in case someone to conduct an operation on the VM in a single step
            Properties = properties;
            var vm = VirtualMachine;
            // create a new client and return this so that properties can be populated automatically
            return new WindowsVirtualMachineClient(properties);
        }

        /// <summary>
        /// Deletes the virtual machine that has context with the client
        /// </summary>
        /// <param name="removeDisks">True if the underlying disks in blob storage should be removed</param>
        /// <param name="removeCloudService">Removes the cloud service container</param>
        /// <param name="removeStorageAccount">The storage account that the vhd is in</param>
        public void DeleteVirtualMachine(bool removeDisks = true, bool removeCloudService = true, bool removeStorageAccount = true)
        {
            // set this if it hasn't been set yet
            PersistentVMRole vm = null;
            if (_vmRole == null)
                vm = VirtualMachine;

            // create the blob client
            string diskName = _vmRole.OSHardDisk.DiskName;
            string storageAccount = ParseBlobDetails(_vmRole.OSHardDisk.MediaLink);
            // create the blob client
            IBlobClient blobClient = new BlobClient(Properties.SubscriptionId, StorageContainerName, storageAccount, Properties.Certificate);
           
            // first delete the virtual machine command
            var deleteVirtualMachine = new DeleteVirtualMachineCommand(Properties)
                                           {
                                               SubscriptionId = Properties.SubscriptionId,
                                               Certificate = Properties.Certificate
                                           };
            try
            {
                deleteVirtualMachine.Execute();
            }
            catch (Exception)
            {
                // should be a 400 here if this is the case then there is only a single role in the deployment - quicker to do it this way!
                var deleteVirtualMachineDeployment = new DeleteVirtualMachineDeploymentCommand(Properties)
                                                         {
                                                             SubscriptionId = Properties.SubscriptionId,
                                                             Certificate = Properties.Certificate
                                                         };
                deleteVirtualMachineDeployment.Execute();
            }
            
            // when this is finished we'll delete the operating system disk - check this as we may need to putin a pause
            // remove the disk association
            DeleteNamedVirtualMachineDisk(diskName);
            // remove the data disks
            DeleteDataDisks(removeDisks ? blobClient : null);
            if (removeDisks)
            {               
                // remove the physical disk
                blobClient.DeleteBlob(StorageFileName);
            }
            // remove the cloud services
            if (removeCloudService)
            {
                // delete the cloud service here
                var deleteCloudService = new DeleteHostedServiceCommand(Properties.CloudServiceName)
                                             {
                                                 SubscriptionId = Properties.SubscriptionId,
                                                 Certificate = Properties.Certificate
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
                                                           SubscriptionId = Properties.SubscriptionId,
                                                           Certificate = Properties.Certificate
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

        private void DeleteDataDisks(IBlobClient client)
        {
            // delete the data disks in the reverse order
            if (_vmRole.HardDisks.HardDiskCollection == null) return;
            for (int i = _vmRole.HardDisks.HardDiskCollection.Count - 1; i >= 0; i--)
            {
                var dataDiskCommand = new DeleteVirtualMachineDiskCommand(_vmRole.HardDisks.HardDiskCollection[i].DiskName)
                                          {
                                              SubscriptionId = Properties.SubscriptionId,
                                              Certificate = Properties.Certificate
                                          };
                dataDiskCommand.Execute();

                int pos = _vmRole.HardDisks.HardDiskCollection[i].MediaLink.LastIndexOf('/');
                string diskFile = _vmRole.HardDisks.HardDiskCollection[i].MediaLink.Substring(pos + 1);
                if(client != null)
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
            var restartCommand = new RestartVirtualMachineCommand(Properties)
            {
                SubscriptionId = Properties.SubscriptionId,
                Certificate = Properties.Certificate
            };
            restartCommand.Execute();
        }

        /// <summary>
        /// Stops the virtual machine instance
        /// </summary>
        public void Stop()
        {
            // start the role up -- this could take a while the previous two operations are fairly lightweight
            // and the provisioning doesn't occur until the role starts not when it is created
            var stopCommand = new StopVirtualMachineCommand(Properties)
            {
                SubscriptionId = Properties.SubscriptionId,
                Certificate = Properties.Certificate
            };
            stopCommand.Execute();
        }

        /// <summary>
        /// Gets thye configuration for the virtual machine
        /// </summary>
        public PersistentVMRole VirtualMachine
        {
            // this should really be an async process
            get
            {
                if (_vmRole != null)
                    return _vmRole;

                var command = new GetWindowsVirtualMachineContextCommand(Properties)
                {
                    SubscriptionId = Properties.SubscriptionId,
                    Certificate = Properties.Certificate
                };
                command.Execute();
                return (_vmRole = command.PersistentVm);
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

        /// <summary>
        /// download rdp file for the windows vm
        /// </summary>
        public void SaveRemoteDesktopFile(string filePath)
        {
            var command = new DownloadWindowsRemoteDesktopCommand(Properties)
            {
                SubscriptionId = Properties.SubscriptionId,
                Certificate = Properties.Certificate
            };
            command.Execute();
            using (var stream = File.Create(filePath))
            {
                stream.Write(command.FileBytes, 0, command.FileLength);
            }
        }

        #endregion

        /// <summary>
        /// Checks whether the necessary properties are populated 
        /// </summary>
        private void EnsureVirtualMachineProperties(WindowsVirtualMachineProperties properties)
        {
            if(properties.Certificate == null || String.IsNullOrEmpty(properties.SubscriptionId) || String.IsNullOrEmpty(properties.CloudServiceName) ||
                String.IsNullOrEmpty(properties.StorageAccountName) || String.IsNullOrEmpty(properties.Location))
                throw new FluentManagementException("Either certificate, subscription id cloud service name or storage account name not present in properties", "CreateWindowsVirtualMachineDeploymentCommand");
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
    }
}
