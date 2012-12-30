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
        private readonly string _subscriptionId;
        private readonly X509Certificate2 _certificate;
        private readonly string _cloudServiceName;

        public VmCreate(string subscriptionId, X509Certificate2 certificate, string cloudServiceName)
        {
            _subscriptionId = subscriptionId;
            _certificate = certificate;
            _cloudServiceName = cloudServiceName;
        }

        #region Implementation of IExecute

        public void Execute()
        {
            var properties = new WindowsVirtualMachineProperties()
                                 {
                                     AdministratorPassword = "Password123",
                                     RoleName = "fluentmanagementvm",
                                     Certificate = _certificate,
                                     Location = LocationConstants.NorthEurope,
                                     UseExistingCloudService = true,
                                     SubscriptionId = _subscriptionId,
                                     CloudServiceName = _cloudServiceName,
                                     PublicEndpoints = new Dictionary<string, int>(){{"web",80}},
                                     VirtualMachineType = VirtualMachineTemplates.WindowsServer2012,
                                     VmSize = VmSize.Small,
                                     StorageAccountName = "elastastorage",
                                     DataDisks = new List<DataVirtualHardDisk>(){new DataVirtualHardDisk(){LogicalDiskSizeInGB = 100}}
                                 };
            var client = new VirtualMachineClient(_subscriptionId, _certificate);
            var newClient = client.CreateNewVirtualMachineFromTemplateGallery(properties);
        }

        public void ParseTokens(string[] args)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
