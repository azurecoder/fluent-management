/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System;
using System.Security.Cryptography.X509Certificates;
using Elastacloud.AzureManagement.Fluent.Clients.Helpers;

namespace Elastacloud.AzureManagement.Fluent.Clients.Interfaces
{
    /// <summary>
    /// Used to describe events which occur in long running client operations
    /// </summary>
    public interface IGenerateUpdateEvent
    {
        /// <summary>
        /// A client event which can be subscribed to to give updates
        /// </summary>
        event FluentClientEventHandler ClientUpdate;
        /// <summary>
        /// Raises a client update event 
        /// </summary>
        void RaiseClientUpdate(int percentage, string description);
    }
}
