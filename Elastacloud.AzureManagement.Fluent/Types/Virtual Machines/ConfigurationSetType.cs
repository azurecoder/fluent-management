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
    /// The type of configuration set that is used with the persistent vm deployment
    /// </summary>
    public enum ConfigurationSetType
    {
        /// <summary>
        /// The network configuration used with endpoints for the vm deployment
        /// </summary>
        NetworkConfiguration,

        /// <summary>
        /// The configuration set used for linux for the deployment
        /// </summary>
        LinuxProvisioningConfiguration,

        /// <summary>
        /// The configuration set used for windows for the deployment
        /// </summary>
        WindowsProvisioningConfiguration
    }
}