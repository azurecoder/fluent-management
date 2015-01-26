/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using Elastacloud.AzureManagement.Fluent.Clients.Interfaces;

namespace Elastacloud.AzureManagement.Fluent.Clients.Helpers
{
    public class GenerateEventClientBase : IGenerateUpdateEvent
    {
        /// <summary>
        /// An event for the update event
        /// </summary>
        public event FluentClientEventHandler ClientUpdate;

        /// <summary>
        /// Raises a client update event 
        /// </summary>
        public void RaiseClientUpdate(int percentage, string description)
        {
            // percentage can't be more than 100%
            if (percentage > 100)
                percentage = 100;
            if(ClientUpdate != null)
                ClientUpdate(percentage, description);
        }
    }
}
