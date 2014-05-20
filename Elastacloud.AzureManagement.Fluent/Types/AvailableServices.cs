/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System;

namespace Elastacloud.AzureManagement.Fluent.Types
{
    /// <summary>
    /// The location of each of the data centres
    /// </summary>
    [Flags]
    public enum AvailableServices
    {
        /// <summary>
        /// Whether compute is available for the subscription in the data centre
        /// </summary>
        Compute = 1,
        /// <summary>
        /// Storage is available in the data centre for this subscription
        /// </summary>
        Storage = 2,
// ReSharper disable once InconsistentNaming
        /// <summary>
        /// Virtual Machines are available in the data centre
        /// </summary>
        PersistentVMRole = 4,
        /// <summary>
        /// High memory instances are available in the data centre which include A6-A9 although A8/A9 are not currently
        /// available in all data centres 
        /// </summary>
        HighMemory = 8
    }
}