/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using Elastacloud.AzureManagement.Fluent.Commands.VirtualMachines;
using Elastacloud.AzureManagement.Fluent.Types.VirtualMachines;
using Elastacloud.AzureManagement.Fluent.VirtualMachines.Classes;

namespace Elastacloud.AzureManagement.Fluent.Clients
{
    /// <summary>
    /// Used to build a virtual machine or to get a handle to an existing virtual machine
    /// </summary>
    public interface IVirtualMachineClient
    {
        /// <summary>
        /// Creates a new virtual machine from a gallery template
        /// </summary>
        /// <param name="properties">Can be any gallery template</param>
        IVirtualMachineClient CreateNewVirtualMachineFromTemplateGallery(WindowsVirtualMachineProperties properties);
        /// <summary>
        /// Deletes the virtual machine that has context with the client
        /// </summary>
        /// <param name="removeDisks">True if the underlying disks in blob storage should be removed</param>
        void DeleteVirtualMachine(bool removeDisks);
        /// <summary>
        /// Restarts the virtual machine instance
        /// </summary>
        void Restart();
        /// <summary>
        /// Stops the virtual machine instance
        /// </summary>
        void Stop();
        /// <summary>
        /// Gets thye configuration for the virtual machine
        /// </summary>
        PersistentVMRole VirtualMachine { get; }
    }
}