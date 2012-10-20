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
    /// The base class configuration set for the 3 types Linux, Windows and Network
    /// </summary>
    public abstract class ConfigurationSet
    {
        /// <summary>
        /// Used to set the type of configuration set used with this vm instance
        /// </summary>
        public abstract ConfigurationSetType ConfigurationSetType { get; }
    }
}