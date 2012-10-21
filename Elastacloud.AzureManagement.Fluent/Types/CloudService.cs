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
    public class CloudService
    {
        /// <summary>
        /// The name of the hosted service
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The hosted service url *.cloudapp.net
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// Where the cloud service is located
        /// </summary>
        public string LocationOrAffinityGroup { get; set; }
        /// <summary>
        /// The date that the cloud service was created
        /// </summary>
        public DateTime Created { get; set; }
        /// <summary>
        /// The date that the cloud service was modified
        /// </summary>
        public DateTime Modified { get; set; }
        /// <summary>
        /// The status of the cloud service e.g. Created, Creating etc.
        /// </summary>
        public CloudServiceStatus Status { get; set; }
        /// <summary>
        /// The list of deployments the cloud service contains
        /// </summary>
        public List<Deployment> Deployments { get; set; }

    }
}