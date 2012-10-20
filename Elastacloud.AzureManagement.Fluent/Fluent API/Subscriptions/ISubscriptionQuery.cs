/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System.Collections.Generic;
using Elastacloud.AzureManagement.Fluent.Types;

namespace Elastacloud.AzureManagement.Fluent.Subscriptions
{
    /// <summary>
    /// All of the possible querying of the accounts available 
    /// </summary>
    public interface ISubscriptionQuery
    {
        /// <summary>
        /// Called when the user needs information about a particular subscription
        /// </summary>
        /// <returns>A SubscriptionInformation object containing details such as core counts and admin addresses etc. </returns>
        SubscriptionInformation GetSubscriptionInformation();

        /// <summary>
        /// A list of subscriber locations for a particular subscription
        /// </summary>
        /// <returns>A generic LocationInformation list containing the details of the location names and their monikers</returns>
        List<LocationInformation> GetSubscriberLocations();
    }
}