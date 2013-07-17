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
            const string userName = "jed";
            
            var serviceClient = new ServiceClient(_applicationFactory.SubscriptionId,
                                                  _applicationFactory.ManagementCertificate,
                                                  _applicationFactory.CloudServiceName);
            //var certificate = serviceClient.CreateServiceCertificate("ASOS R-MPI", _applicationFactory.Password, @"C:\Projects\Asos\keys");

            var propertiesList = new List<LinuxVirtualMachineProperties>();
            for(int i = 43; i <= 43; i++)
            {
                var properties = new LinuxVirtualMachineProperties()
                {
                    UserName = userName,
                    AdministratorPassword = _applicationFactory.Password,
                    HostName = String.Format("{0}-{1}", _applicationFactory.DeploymentName, i),
                    RoleName = String.Format("{0}-{1}", _applicationFactory.DeploymentName, i),
                    CloudServiceName = _applicationFactory.CloudServiceName,
                    PublicEndpoints = new List<InputEndpoint>(new[] {new InputEndpoint() 
                                     { 
                                         EndpointName = "SSH", 
                                         LocalPort = 22,
                                         Port = 6185 + i,
                                         Protocol = Protocol.TCP
                                     }}),
                    CustomTemplateName = "asos-r-allpackages",
                    DeploymentName = _applicationFactory.DeploymentName,
                    VmSize = VmSize.ExtraLarge,
                    StorageAccountName = _applicationFactory.RoleName
                };
                propertiesList.Add(properties);
            }
            
           

            // set up the service certificate first of all 
            var client = new LinuxVirtualMachineClient(_applicationFactory.SubscriptionId, _applicationFactory.ManagementCertificate);

            var certificate = new X509Certificate2(@"C:\Projects\Asos\keys\25917D3D71E06109B67901B2357C608B00F2187B.pfx", "asosb!gcompute1302", X509KeyStorageFlags.Exportable);

            var model = new ServiceCertificateModel()
                {
                    Password = _applicationFactory.Password,
                    ServiceCertificate = certificate
                };
            
                client.AddRolesToExistingDeployment(
                    propertiesList,
                    _applicationFactory.CloudServiceName, model);
        }

        public void ParseTokens(string[] args)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
