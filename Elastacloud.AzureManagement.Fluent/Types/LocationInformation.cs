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
    }
}