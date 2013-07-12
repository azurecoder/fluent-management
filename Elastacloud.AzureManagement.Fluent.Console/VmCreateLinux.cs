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
            var serviceClient = new ServiceClient(_applicationFactory.SubscriptionId,
                                                  _applicationFactory.ManagementCertificate,
                                                  _applicationFactory.CloudServiceName);
            var certificate = serviceClient.CreateServiceCertificate("ASOS R-MPI", "password", @"C:\Projects\Asos");
            var keypairs = new SSHKey(KeyType.KeyPair)
                {
                    FingerPrint = certificate.Thumbprint,
                    Path = "/home/moodyballs101/.ssh/authorized_keys"
                };
            var publickey = new SSHKey(KeyType.PublicKey)
                {
                    FingerPrint = certificate.Thumbprint,
                    Path = "/home/moodyballs101/.ssh/id_rsa"
                };
            var properties1 = new LinuxVirtualMachineProperties()
                                 {
                                     UserName = "moodyballs101",
                                     AdministratorPassword = _applicationFactory.Password,
                                     HostName = "asosmpiplus1",
                                     RoleName = "asosmpiplus1",
                                     CloudServiceName = _applicationFactory.CloudServiceName,
                                     PublicEndpoints = new List<InputEndpoint>(new[] {new InputEndpoint() 
                                     { 
                                         EndpointName = "SSH", 
                                         LocalPort = 22,
                                         Port = 6185,
                                         Protocol = Protocol.TCP
                                     }}),
                                     CustomTemplateName = "asosrmpi",
                                     DeploymentName = "asosmpiplus",
                                     VmSize = VmSize.Small,
                                     StorageAccountName = "rmpi",
                                     DisableSSHPasswordAuthentication = true,
                                     PublicKeys = new List<SSHKey>(new[]{publickey}),
                                     KeyPairs = new List<SSHKey>(new[]{keypairs})
                                 };
            var properties2 = new LinuxVirtualMachineProperties()
                {
                    UserName = "moodyballs101",
                    AdministratorPassword = _applicationFactory.Password,
                    RoleName = "asosmpiplus2",
                    HostName = "asosmpiplus2",
                    CloudServiceName = _applicationFactory.CloudServiceName,
                    PublicEndpoints = new List<InputEndpoint>(new[]
                        {
                            new InputEndpoint()
                                {
                                    EndpointName = "SSH",
                                    LocalPort = 22,
                                    Port = 6186,
                                    Protocol = Protocol.TCP
                                }
                        }),
                    CustomTemplateName = "rmpislave",
                    DeploymentName = "asosmpiplus",
                    VmSize = VmSize.Small,
                    StorageAccountName = "rmpi",
                    DisableSSHPasswordAuthentication = true,
                    PublicKeys = new List<SSHKey>(new[] { publickey }),
                    KeyPairs = new List<SSHKey>(new[] { keypairs })
                };
            var properties3 = new LinuxVirtualMachineProperties()
            {
                UserName = "moodyballs101",
                AdministratorPassword = _applicationFactory.Password,
                RoleName = "asosmpiplus3",
                HostName = "asosmpiplus3",
                CloudServiceName = _applicationFactory.CloudServiceName,
                PublicEndpoints = new List<InputEndpoint>(new[]
                        {
                            new InputEndpoint()
                                {
                                    EndpointName = "SSH",
                                    LocalPort = 22,
                                    Port = 6187,
                                    Protocol = Protocol.TCP
                                }
                        }),
                CustomTemplateName = "rmpislave",
                DeploymentName = "asosmpiplus",
                VmSize = VmSize.Small,
                StorageAccountName = "rmpi",
                DisableSSHPasswordAuthentication = true,
                PublicKeys = new List<SSHKey>(new[] { publickey }),
                KeyPairs = new List<SSHKey>(new[] { keypairs })
            };

            // set up the service certificate first of all 
            var client = new LinuxVirtualMachineClient(_applicationFactory.SubscriptionId, _applicationFactory.ManagementCertificate);

            var model = new ServiceCertificateModel()
                {
                    Password = _applicationFactory.Password,
                    ServiceCertificate = certificate
                };
            var newClient =
                client.CreateNewVirtualMachineDeploymentFromTemplateGallery(
                    new List<LinuxVirtualMachineProperties>(new[] {properties1, properties2}),
                    _applicationFactory.CloudServiceName, model);
        }

        public void ParseTokens(string[] args)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
