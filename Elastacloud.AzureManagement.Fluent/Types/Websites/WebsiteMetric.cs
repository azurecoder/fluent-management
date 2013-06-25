/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System;

namespace Elastacloud.AzureManagement.Fluent.Types.Websites
{
    /// <summary>
    /// Contains the basic metric for a WAWS
    /// </summary>
    public class WebsiteMetric
    {
        /// <summary>
        /// The display name which is shown in the windows azure portal
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// The name of the underlying metric (without spaces)
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The start time that the aggregation is requested
        /// </summary>
        public DateTime StartTime { get; set; }
        /// <summary>
        /// The end time that the aggregation is requested
        /// </summary>
        public DateTime EndTime { get; set; }
        /// <summary>
        /// The total volume of data in units aggregated over that period
        /// </summary>
        public int Total { get; set; }
        /// <summary>
        /// The units that the sample is in - Bytes etc.
        /// </summary>
        public string Units { get; set; }
    }
}
