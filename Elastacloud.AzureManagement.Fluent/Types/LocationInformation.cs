﻿/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System.Collections.Generic;

namespace Elastacloud.AzureManagement.Fluent.Types
{
    /// <summary>
    /// The location of each of the data centres
    /// </summary>
    public class LocationInformation
    {
        /// <summary>
        /// What the name is internally to Windows Azure
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// How this name is displayed to the portal
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// Gets a list of all of the availabel virtual machine role sizes
        /// </summary>
        public List<VmSize> VirtualMachineRolesSizes { get; set; }
        /// <summary>
        /// Gets a list of all of the web/worker role sizes
        /// </summary>
        public List<VmSize> WebWorkerRolesSizes { get; set; }
        /// <summary>
        /// The services available to the location in question
        /// </summary>
        public AvailableServices AvailableServices { get; set; }
 
    }
}