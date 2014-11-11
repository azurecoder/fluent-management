/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System.Collections.Generic;
using Elastacloud.AzureManagement.Fluent.Commands.VirtualMachines;
using Elastacloud.AzureManagement.Fluent.Types.VirtualMachines;
using Elastacloud.AzureManagement.Fluent.Types.VirtualNetworks;
using Elastacloud.AzureManagement.Fluent.VirtualMachines.Classes;

namespace Elastacloud.AzureManagement.Fluent.Clients.Interfaces
{
    /// <summary>
    /// Used to build a virtual machine or to get a handle to an existing virtual machine
    /// </summary>
    public interface IVirtualMachineClient
    {
        /// <summary>
        /// Deletes the virtual machine that has context with the client
        /// </summary>
        /// <param name="removeDisks">True if the underlying disks in blob storage should be removed</param>
        /// <param name="removeUnderlyingBlobs">Whether or not remove the blob as well as the OS disk</param>
        /// <param name="removeCloudService">Removes the cloud service container</param>
        /// <param name="removeStorageAccount">The storage account that the vhd is in</param>
        void DeleteVirtualMachine(bool removeDisks, bool removeUnderlyingBlobs, bool removeCloudService, bool removeStorageAccount);
        /// <summary>
        /// Deletes a vm disk if a name is known
        /// </summary>
        /// <param name="name">The name of the vm disk</param>
        void DeleteNamedVirtualMachineDisk(string name);
        /// <summary>
        /// Restarts the virtual machine instance
        /// </summary>
        void Restart();
        /// <summary>
        /// Stops the virtual machine instance
        /// </summary>
        void Stop();
       
        /// <summary>
        /// Gets the container that the storage blob resides in 
        /// </summary>
        string StorageContainerName { get; }
        /// <summary>
        /// The name of the blob which is stored for the vm
        /// </summary>
        string StorageFileName { get; }
        /// <summary>
        /// Cleans up any disks which don't have an attached VM
        /// </summary>
        void CleanupUnattachedDisks();
        /// <summary>
        /// Gets a list of hosts, internal ip addresses and other things
        /// </summary>
        List<VmHost> GetAllInternalHostDetails();
        /// <summary>
        /// Gets the virtual networks available in the current subscription
        /// </summary>
        List<VirtualNetworkSite> GetAvailableVirtualNetworks();
        // put a placeholder in here to do the same for availability sets
    }
}