/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System.Collections.Generic;

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
}
