using System;
using System.Collections.Generic;
using System.IO;
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
using Elastacloud.AzureManagement.ScriptMapper.Linux;

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
            var session = new LinuxSession("relastacloud.cloudapp.net", 6187, "richard", "elastab!gcompute123", true,
                                           @"C:\Projects\Elastacloud\Elastacloud.Fluent.ClusterManager\Elastacloud.Fluent.ClusterManager\bin\Debug\keys2\93CDF657CD33DFFFA3D233881984354315759A99.pem");
            session.ExecuteCommand("ssh-keygen");
        }

        public void ParseTokens(string[] args)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
