/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/
using System;

namespace Elastacloud.AzureManagement.Fluent.Clients.Helpers
{
    /// <summary>
    /// Used to describe a change in state of a client process - associated with a particular long running method
    /// </summary>
    public class FluentManagementEventArgs : EventArgs
    {
        /// <summary>
        /// Created with a percentage and a descriptive string describing the event
        /// </summary>
        public FluentManagementEventArgs(int percentageUpdate, string contextString)
        {
            PercentageUpdated = percentageUpdate;
            Description = contextString;
        }
        /// <summary>
        /// The percentage of the event that has been updated
        /// </summary>
        public int PercentageUpdated { get; private set; }
        /// <summary>
        /// As description of said event
        /// </summary>
        public string Description { get; private set; }
    }
    /// <summary>
    /// An event handler which is used to generate a perentage status update and associated description
    /// </summary>
    public delegate void FluentClientEventHandler(int percentage, string description);
}