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
using System.Linq;
using System.Text;

namespace Elastacloud.AzureManagement.Fluent.Types
{
    /// <summary>
    /// Used to refer to a deployment under a cloud service
    /// </summary>
    public class Deployment
    {
        /// <summary>
        /// The name of the deployment
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The slot taken by the deployment
        /// </summary>
        public DeploymentSlot Slot { get; set; }
        /// <summary>
        /// The total role instance belonging to the deployment
        /// </summary>
        public int TotalRoleInstanceCount { get; set; }
        /// <summary>
        /// The roleinstances associated with the deployment
        /// </summary>
        public List<RoleInstance> RoleInstances { get; set; }
        /// <summary>
        /// The virtual IP address of the deployment
        /// </summary>
        public string IpAddress { get; set; }
         /// <summary>
        /// The slot taken by the deployment
        /// </summary>
        public string DeploymentID { get; set; }
        }
    }
}
