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
using System.Text;
using System.Threading.Tasks;
using Elastacloud.AzureManagement.Fluent.Types.VirtualNetworks;
using Elastacloud.AzureManagement.Fluent.VirtualNetwork;

namespace Elastacloud.AzureManagement.Fluent.Clients.Interfaces
{
    interface IVirtualNetworkingClient
    {
        /// <summary>
        /// Gets the virtual networks available in the current subscription
        /// </summary>
        IEnumerable<VirtualNetworkingUtils.VirtualNetwork> GetAvailableVirtualNetworks();
        /// <summary>
        /// Creates an empty VNET to add address ranges and subnets to
        /// </summary>
        string AddSubnetToAddressRange(string networkName, string addressRange, string subnetName);
        /// <summary>
        /// Checks to see whether an IP address in a virtual network is available
        /// </summary>
        AvailableIpAddresses IsIpAddressAvailable(string vnet, string ipToCheck);
        /// <summary>
        /// Gets all of the networking config for the 
        /// </summary>
        string GetAllNetworkingConfig();
        /// <summary>
        /// Removes a subnet from the network configuration 
        /// </summary>
        void RemoveSubnet(string networkName, string subnetName);
    }
}
