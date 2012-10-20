/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using Elastacloud.AzureManagement.Fluent.Commands.VirtualMachines;
using Elastacloud.AzureManagement.Fluent.Types;

namespace Elastacloud.AzureManagement.Fluent.VirtualMachines
{
    /// <summary>
    /// Used to deploy a virtual machine from a pre-defined template
    /// </summary>
    public interface IVirtualMachineDeployment
    {
        /// <summary>
        /// Selects the size of the VM to provision
        /// </summary>
        /// <param name="size">the size of the VM to provision</param>
        /// <returns>An IVirtualMachineDeployment interface</returns>
        IVirtualMachineDeployment WithVmOfSize(VmSize size);
        /// <summary>
        /// The type of deployment that is being made - SQL, AD, stored image, etc.
        /// </summary>
        /// <param name="templates">The deployment type</param>
        /// <returns>A IVirtualMachineDeployment interface</returns>
        IVirtualMachineDeployment WithDeploymentType(VirtualMachineTemplates templates);
        /// <summary>
        /// The setter for the storage account to hold the VHDs for the data disk and the OS disk
        /// </summary>
        /// <param name="storageAccount">the storage account used to store the VHDs</param>
        /// <returns>A IVirtualMachineDeployment interface</returns>
        IVirtualMachineDeployment WithStorageAccountForVhds(string storageAccount);
        /// <summary>
        /// The cloud service account which the virtual machine is being deployed to
        /// </summary>
        /// <param name="name">the name of the cloud service</param>
        /// <returns>A IVirtualMachineDeployment interface</returns>
        IVirtualMachineDeployment AddToExistingCloudServiceWithName(string name);
        /// <summary>
        /// Used to deploy the virtual machine 
        /// </summary>
        void Deploy();
    }
}