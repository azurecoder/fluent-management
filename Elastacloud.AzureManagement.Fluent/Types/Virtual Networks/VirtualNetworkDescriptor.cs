using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elastacloud.AzureManagement.Fluent.Types.VirtualNetworks
{
    /// <summary>
    /// A class which describes the virtual network requirements for creating a virtual machine
    /// </summary>
    public class VirtualNetworkDescriptor
    {
        /// <summary>
        /// The name of the virtual network which is used in the virtual machine creation
        /// </summary>
        public string VirtualNetworkName { get; set; }
        /// <summary>
        /// The name of the subnet that the virtual machine should be joined to
        /// </summary>
        public string SubnetName { get; set; }
    }
}
