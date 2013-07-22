using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Elastacloud.AzureManagement.Fluent.Clients;
using Elastacloud.AzureManagement.Fluent.Clients.Helpers;
using Elastacloud.AzureManagement.Fluent.Commands.VirtualMachines;
using Elastacloud.AzureManagement.Fluent.Helpers;
using Elastacloud.AzureManagement.Fluent.Helpers.PublishSettings;
using Elastacloud.AzureManagement.Fluent.Linq;
using Elastacloud.AzureManagement.Fluent.Types;
using Elastacloud.AzureManagement.Fluent.Types.VirtualMachines;
using Elastacloud.AzureManagement.Fluent.VirtualMachines.Classes;

namespace Elastacloud.AzureManagement.Fluent.Console
{
    public class VmCreateLinux : IExecute
    {
        private readonly ApplicationFactory _applicationFactory;

        public VmCreateLinux(ApplicationFactory factory)
        {
            _applicationFactory = factory;
        }

        #region Implementation of IExecute

        public void Execute()
        {
            var input = new LinuxVirtualMachineClient(_applicationFactory.SubscriptionId,
                                                      _applicationFactory.ManagementCertificate);
            input.CleanupUnattachedDisks();

            var inputs = new LinqToAzureInputs()
            {
                ManagementCertificateThumbprint = _applicationFactory.ManagementCertificate.Thumbprint,
                SubscriptionId = _applicationFactory.SubscriptionId
            };
            var queryableVirtualMachines = new LinqToAzureOrderedQueryable<VirtualMachineProperties>(inputs);
            // build up a filtered query to check the new account
            var query = from vms in queryableVirtualMachines
                        where vms.CloudServiceName == "rmpicloud3"
                        select vms;
            var myAccount = query.First();
    

            
        }

        public void ParseTokens(string[] args)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
