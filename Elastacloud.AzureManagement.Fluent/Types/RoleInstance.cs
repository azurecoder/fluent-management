/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System;
using System.Collections.Generic;

namespace Elastacloud.AzureManagement.Fluent.Types
{
    /// <summary>
    /// Contains the hosted service details 
    /// </summary>
    public class RoleInstance
    {
        /// <summary>
        /// The unique name of the role instance 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The ip address of the roleinstance
        /// </summary>
        public string IpAddress { get; set; }
        /// <summary>
        /// The status of the role instance
        /// </summary>
        public RoleInstanceStatus Status { get; set; }
        /// <summary>
        /// The size of the role instance
        /// </summary>
        public VmSize Size { get; set; }
    }
}