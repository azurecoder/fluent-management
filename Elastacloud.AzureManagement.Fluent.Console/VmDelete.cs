using System;
using Elastacloud.AzureManagement.Fluent.Clients;
using Elastacloud.AzureManagement.Fluent.VirtualMachines.Classes;

namespace Elastacloud.AzureManagement.Fluent.Console
{
    public class VmDelete : IExecute
    {
        private readonly ApplicationFactory _applicationFactory;

        public VmDelete(ApplicationFactory factory)
        {
            _applicationFactory = factory;
        }

        #region Implementation of IExecute

        public void Execute()
        {
            var properties = new WindowsVirtualMachineProperties()
            {
                AdministratorPassword = _applicationFactory.Password,
                RoleName = _applicationFactory.RoleName,
                Certificate = _applicationFactory.ManagementCertificate,
                SubscriptionId = _applicationFactory.SubscriptionId,
                CloudServiceName = _applicationFactory.CloudServiceName,
                DeploymentName = _applicationFactory.DeploymentName
            };
            var client = new WindowsVirtualMachineClient(properties);
            var vm = client.VirtualMachine;
            client.DeleteVirtualMachine(true, true);
            
            System.Console.WriteLine("Deleted virtual machine + disks");
        }

        public void ParseTokens(string[] args)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
