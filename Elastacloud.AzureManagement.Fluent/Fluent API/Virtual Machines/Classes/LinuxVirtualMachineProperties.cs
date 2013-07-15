/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System.Collections.Generic;
using Elastacloud.AzureManagement.Fluent.Types;
using Elastacloud.AzureManagement.Fluent.Types.VirtualMachines;

namespace Elastacloud.AzureManagement.Fluent.VirtualMachines.Classes
{
    /// <summary>
    /// This contains the virtual machine properties to make the deployment
    /// </summary>
    public class LinuxVirtualMachineProperties : VirtualMachineProperties
    {
        /// <summary>
        /// Creates a linux virtual machine properties
        /// </summary>
        public LinuxVirtualMachineProperties()
        {
            PublicKeys = new List<SSHKey>();
            KeyPairs = new List<SSHKey>();
        }
        /// <summary>
        /// a list of the public keys that are available to deploy to the linux VMs
        /// </summary>
        internal List<SSHKey> PublicKeys { get; set; }
        /// <summary>
        /// A list of the key pairs available to linux vms
        /// </summary>
        internal List<SSHKey> KeyPairs { get; set; }
        /// <summary>
        /// The name of the linux host
        /// </summary>
        public string HostName { get; set; }
        /// <summary>
        /// The username of the linux user
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// The password auth and whether to only use SSH
        /// </summary>
        internal bool DisableSSHPasswordAuthentication { get; set; }
    }
}
