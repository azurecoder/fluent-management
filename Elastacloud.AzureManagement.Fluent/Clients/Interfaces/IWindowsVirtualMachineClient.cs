/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/
using Elastacloud.AzureManagement.Fluent.Types.VirtualMachines;
using Elastacloud.AzureManagement.Fluent.VirtualMachines.Classes;

namespace Elastacloud.AzureManagement.Fluent.Clients.Interfaces
{
    public interface IWindowsVirtualMachineClient : IVirtualMachineClient
    {
        /// <summary>
        /// Returns the properties of the associated virtual machine
        /// </summary>
        WindowsVirtualMachineProperties Properties { get; set; }
        /// <summary>
        /// Gets thye configuration for the virtual machine
        /// </summary>
        PersistentVMRole VirtualMachine { get; }
        /// <summary>
        /// Creates a new virtual machine from a gallery template
        /// </summary>
        /// <param name="properties">Can be any gallery template</param>
        IVirtualMachineClient CreateNewVirtualMachineFromTemplateGallery(WindowsVirtualMachineProperties properties);
    }
}
