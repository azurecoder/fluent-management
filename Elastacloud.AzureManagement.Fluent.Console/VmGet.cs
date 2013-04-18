using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Elastacloud.AzureManagement.Fluent.Clients;
using Elastacloud.AzureManagement.Fluent.Commands.VirtualMachines;
using Elastacloud.AzureManagement.Fluent.Helpers;
using Elastacloud.AzureManagement.Fluent.Types;
using Elastacloud.AzureManagement.Fluent.Types.VirtualMachines;
using Elastacloud.AzureManagement.Fluent.VirtualMachines.Classes;

namespace Elastacloud.AzureManagement.Fluent.Console
{
    public class VmGet : IExecute
    {
        private readonly ApplicationFactory _applicationFactory;

        public VmGet(ApplicationFactory factory)
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

            System.Console.WriteLine("Virtual machine client");
            System.Console.WriteLine("======================");
            System.Console.WriteLine("Role name: {0}", vm.RoleName);
            System.Console.WriteLine("Role size: {0}", vm.RoleSize.ToString());

            string rdpFile = Path.Combine(_applicationFactory.PublishSettingsRoot, "vm.rdp");
            client.SaveRemoteDesktopFile(rdpFile);
            System.Console.WriteLine("RDP file saved to: {0}", rdpFile);
        }

        public void ParseTokens(string[] args)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
