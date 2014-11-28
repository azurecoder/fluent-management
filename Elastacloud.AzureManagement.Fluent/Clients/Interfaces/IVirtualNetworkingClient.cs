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
        void CreateNamedVirtualNetwork(string networkName);

        /// <summary>
        /// Adds an address range and associated subnets to an existing VNET
        /// </summary>
        List<VirtualNetworkSite> AddNewAddressRange(string name, VirtualNetworkSite site);
    }
}
