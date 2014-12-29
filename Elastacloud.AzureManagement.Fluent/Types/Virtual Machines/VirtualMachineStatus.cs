/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

namespace Elastacloud.AzureManagement.Fluent.Types.VirtualMachines
{
    /// <summary>
    /// Holds the status of a virtual machine
    /// </summary>
    public class VirtualMachineStatus
    {
        /// <summary>
        /// The previous role instance status of the virtual machine
        /// </summary>
        public RoleInstanceStatus OldStatus { get; set; }
        /// <summary>
        /// The new status that a virtual machine will have to go through
        /// </summary>
        public RoleInstanceStatus NewStatus { get; set; }
        /// <summary>
        /// The name of the virtual machine and the hostname (normally)
        /// </summary>
        public string VirtualMachineInstanceName { get; set; }
        /// <summary>
        /// The cloud service to which the virtual machine belongs to 
        /// </summary>
        public string CloudService { get; set; }
    }
}
