/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System.Collections.Generic;
using System.Net;

namespace Elastacloud.AzureManagement.Fluent.Types.VirtualNetworks
{
    public class VirtualNetworkSite
    {
        public string Name { get; set; }
        public string Label { get; set; }
        public string Id { get; set; }
        public string Location { get; set; }
        public string State { get; set; }
        public List<Subnet> Subnets { get; set; }
        public List<AddressSpace> AddressSpaces { get; set; }
    }

    public class Subnet
    {
        public string Name { get; set; }
        public string CidrAddressRange { get; set; }
    }
    /// <summary>
    /// Contains the address space and CIDR 
    /// </summary>
    public class AddressSpace
    {
        public string Name { get; set; }
        public string CidrAddressRange { get; set; }
    }
    /// <summary>
    /// REturns the available ip addresses from a service management request
    /// </summary>
    public class AvailableIpAddresses
    {
        /// <summary>
        /// Checks to see whether the requested ip address is available
        /// </summary>
        public bool IpIsAvailable { get; set; }
        /// <summary>
        /// The ip that has been requested in the service management request
        /// </summary>
        public IPAddress RequestedIp { get; set; }
        /// <summary>
        /// The ips that are available if the requested address is not
        /// </summary>
        public List<IPAddress> AvailableIps { get; set; }
    }
}
