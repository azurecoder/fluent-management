﻿using System;
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
            var props = new List<LinuxVirtualMachineProperties>();
            for (int i = 1; i <= 7; i++)
            {
                var properties = new LinuxVirtualMachineProperties()
                {
                    CloudServiceName = "asos-yarn-spark2",
                    HostName = "spark-node" + i,
                    RoleName = "spark-node" + i,
                    UserName = "azurecoder",
                    AdministratorPassword = "AsosSp@rk20148yJed",
                    DeploymentName = "spark-master",
                    CustomTemplateName = "Azure-Data-Analysis",
                    StorageAccountName = "rmpi",
                    PublicEndpoints = new List<InputEndpoint>()
                    {
                        new InputEndpoint()
                        {
                            EndpointName = "SSL",
                            LocalPort = 22,
                            Port = 21 + i,
                            Protocol = Protocol.TCP
                        }
                    },
                    VmSize = VmSize.A7
                };
                //var linux1 = new LinuxVirtualMachineClient(_applicationFactory.SubscriptionId, _applicationFactory.ManagementCertificate)
                //{
                //    Properties = new List<LinuxVirtualMachineProperties>() { properties }
                //};
                //linux1.DeleteVirtualMachine(true, true, false, false);
                props.Add(properties);
            }
            //var settings =
            //    new PublishSettingsExtractor(@"C:\Projects\ASOS Big Compute-12-30-2013-credentials.publishsettings");
            //var cert = settings.AddPublishSettingsToPersonalMachineStore();
            var linux = new LinuxVirtualMachineClient(_applicationFactory.SubscriptionId, _applicationFactory.ManagementCertificate);
            //linux.AddRolesToExistingDeployment(props, "asos-yarn-spark", null);
            linux.CreateNewVirtualMachineDeploymentFromTemplateGallery(props, "asos-yarn-spark2");
        }

        public void ParseTokens(string[] args)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
