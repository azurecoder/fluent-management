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
    /// An enum value which refers to the state of the disk in virtual machine - this will affect the performance of the disk
    /// </summary>
    public enum HostCaching
    {
        /// <summary>
        /// This is the the default where there is no caching on the hard drive
        /// </summary>
        None,

        /// <summary>
        /// A readonly value for the virtual hard drive 
        /// </summary>
        ReadOnly,

        /// <summary>
        /// A readwrite value for the virtual hard drive
        /// </summary>
        ReadWrite
    }
}