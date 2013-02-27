using System;
using System.Collections.Generic;
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
    public class VmCreate : IExecute
    {
        private readonly ApplicationFactory _applicationFactory;

        public VmCreate(ApplicationFactory factory)
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
                                     Location = LocationConstants.NorthEurope,
                                     UseExistingCloudService = true,
                                     SubscriptionId = _applicationFactory.SubscriptionId,
                                     CloudServiceName = _applicationFactory.CloudServiceName,
                                     PublicEndpoints = new Dictionary<string, int>(){{"web",80}},
                                     VirtualMachineType = VirtualMachineTemplates.WindowsServer2008R2SP1,
                                     VmSize = VmSize.Small,
                                     StorageAccountName = "elastastorage",
                                     DataDisks = new List<DataVirtualHardDisk>(){new DataVirtualHardDisk(){LogicalDiskSizeInGB = 100}}
                                 };
            var client = new WindowsVirtualMachineClient(_applicationFactory.SubscriptionId, _applicationFactory.ManagementCertificate);
            var newClient = client.CreateNewVirtualMachineFromTemplateGallery(properties);
        }

        public void ParseTokens(string[] args)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
