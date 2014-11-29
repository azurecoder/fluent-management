/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Elastacloud.AzureManagement.Fluent.Clients.Interfaces;
using Elastacloud.AzureManagement.Fluent.Commands.VirtualMachines;
using Elastacloud.AzureManagement.Fluent.Types.VirtualNetworks;
using Elastacloud.AzureManagement.Fluent.VirtualNetwork;

namespace Elastacloud.AzureManagement.Fluent.Clients
{
    /// <summary>
    /// Creates a virtual networking client which has the capability to return information about the vnet
    /// and add things to it
    /// </summary>
    public class VirtualNetworkClient : IVirtualNetworkingClient
    {
        public VirtualNetworkClient(string subscriptionId, X509Certificate2 managementCertificate)
        {
            SubscriptionId = subscriptionId;
            ManagementCertificate = managementCertificate;
        }

        public X509Certificate2 ManagementCertificate { get; set; }

        public string SubscriptionId { get; set; }
        /// <summary>
        /// Gets the available virtual networks in the correct order binding the address ranges to the subnets
        /// </summary>
        public IEnumerable<VirtualNetworkingUtils.VirtualNetwork> GetAvailableVirtualNetworks()
        {
            var command = new ListVirtualNetworksCommand()
            {
                SubscriptionId = SubscriptionId,
                Certificate = ManagementCertificate
            };
            command.Execute();
            return VirtualNetworkingUtils.ConvertVNetToHierarchicalModel(command.VirtualNetworks);
        }

        public void CreateNamedVirtualNetwork(string networkName)
        {
            throw new NotImplementedException();
        }

        public List<VirtualNetworkSite> AddNewAddressRange(string name, VirtualNetworkSite range)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks to see whether an IP address in a virtual network is available
        /// </summary>
        public AvailableIpAddresses IsIpAddressAvailable(string vnet, string ipToCheck)
        {
            var command = new GetAvailableIpAddressesCommand(vnet, ipToCheck)
            {
                SubscriptionId = SubscriptionId,
                Certificate = ManagementCertificate
            };
            command.Execute();
            return command.IpAddressCheck;
        }
    }
}
