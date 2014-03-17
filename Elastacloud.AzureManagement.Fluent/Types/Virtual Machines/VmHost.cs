using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elastacloud.AzureManagement.Fluent.Types.VirtualMachines
{
    /// <summary>
    /// Contains the private information for a vm host in a cloud service
    /// </summary>
    public class VmHost
    {
        /// <summary>
        /// The hostname of the vm which you can get from the hostname command
        /// </summary>
        public string HostName { get; internal set; }
        /// <summary>
        /// The equivalent rolename of the vm instance
        /// </summary>
        public string RoleName { get; internal set; }
        /// <summary>
        /// The internal Ip address of the host
        /// </summary>
        public string IpAddress { get; internal set; }

    }
}
