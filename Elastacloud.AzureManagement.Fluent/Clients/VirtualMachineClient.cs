using System;
using System.Security.Cryptography.X509Certificates;
using Elastacloud.AzureManagement.Fluent.Commands.Services;
using Elastacloud.AzureManagement.Fluent.Commands.VirtualMachines;
using Elastacloud.AzureManagement.Fluent.Types.Exceptions;
using Elastacloud.AzureManagement.Fluent.VirtualMachines.Classes;

namespace Elastacloud.AzureManagement.Fluent.Clients
{
    public class VirtualMachineClient : IVirtualMachineClient
    {
        // the name of the cloud service to look up 
        private string _cloudServiceName;

        /// <summary>
        /// Constructs a VirtualMachineClient and will get the details of a virtual machine given a cloud service
        /// </summary>
        /// <param name="subscriptionId">the subscription id </param>
        /// <param name="certificate">A management certificate for the subscription</param>
        /// <param name="cloudServiceName">A cloud service which is in the subscription and contains the virtual machine</param>
        public VirtualMachineClient(string subscriptionId, X509Certificate2 certificate, string cloudServiceName = null)
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
        public VirtualMachineClient(WindowsVirtualMachineProperties properties)
        {
            Properties = properties;
        }

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
            // create a new client and return this so that properties can be populated automatically
            return new VirtualMachineClient(properties);
        }

        /// <summary>
        /// Deletes the virtual machine that has context with the client
        /// </summary>
        /// <param name="removeDisks">True if the underlying disks in blob storage should be removed</param>
        public void DeleteVirtualMachine(bool removeDisks)
        {
            throw new System.NotImplementedException();
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
    }
}
