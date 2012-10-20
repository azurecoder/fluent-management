/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/
namespace Elastacloud.AzureManagement.Fluent.VirtualMachines
{
    /// <summary>
    /// Used to select whether a query or a 
    /// </summary>
    public interface IVirtualMachineActivity
    {
        /// <summary>
        /// Allows a query to be made against the virtual machines catalog
        /// </summary>
        /// <returns>An IVirtualMachineQuery interface</returns>
        IVirtualMachineQuery QueryVirtualMachineManagement();
        /// <summary>
        /// Creates a virtual machine deployment
        /// </summary>
        /// <returns>An IVirtualMachineDeployment interface</returns>
        IVirtualMachineDeployment CreateVirtualMachineDeployment();
    }
}