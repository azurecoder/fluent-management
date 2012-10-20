/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System.Net;
using Elastacloud.AzureManagement.Fluent.Commands.Parsers;
using Elastacloud.AzureManagement.Fluent.Commands.Services;
using Elastacloud.AzureManagement.Fluent.Types;

namespace Elastacloud.AzureManagement.Fluent.Commands.Subscriptions
{
    /// <summary>
    /// Gets all of the properties associated with a particular subscription
    /// </summary>
    internal class GetSubscriptionCommand : ServiceCommand
    {
        /// <summary>
        /// Used to invoke the HTTP GET command
        /// </summary>
        internal GetSubscriptionCommand()
        {
            HttpVerb = HttpVerbGet;
        }

        /// <summary>
        /// Contains the property information for the subscription as an object representation
        /// </summary>
        internal SubscriptionInformation SubscriptionInformation { get; set; }

        /// <summary>
        /// Process the response and sets the subscription information from the HTTP Body
        /// </summary>
        protected override void ResponseCallback(HttpWebResponse webResponse)
        {
            SubscriptionInformation = Parse(webResponse, BaseParser.GetSubscriptionParser);
            SitAndWait.Set();
        }
    }
}