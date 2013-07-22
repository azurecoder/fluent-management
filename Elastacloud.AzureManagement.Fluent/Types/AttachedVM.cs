/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/


namespace Elastacloud.AzureManagement.Fluent.Types
{
    /// <summary>
    /// The attached VM for the Virtual Disk
    /// </summary>
    public class AttachedVM
    {
        /// <summary>
        /// The cloud service the disk is attached to 
        /// </summary>
        public string CloudServiceName { get; set; }
        /// <summary>
        /// The deployment the disk is attached to 
        /// </summary>
        public string DeploymentName { get; set; }
        /// <summary>
        /// The role name of the virtual machine
        /// </summary>
        public string RoleName { get; set; }
    }
}