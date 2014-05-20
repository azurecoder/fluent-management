/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System.Collections.Generic;
using System.Net;
using Elastacloud.AzureManagement.Fluent.Commands.Parsers;
using Elastacloud.AzureManagement.Fluent.Commands.Services;
using Elastacloud.AzureManagement.Fluent.Types;

namespace Elastacloud.AzureManagement.Fluent.Commands.Subscriptions
{
    /// <summary>
    /// Gets the available locations for this subscription - this is relevant where Microsoft provide free 
    /// </summary>
    internal class GetSubscriberLocationsCommand : ServiceCommand
    {
        // https://management.core.windows.net/<subscription-id>/locations
        internal GetSubscriberLocationsCommand()
        {
            ServiceType = "locations";
            HttpVerb = HttpVerbGet;
            AdditionalHeaders["x-ms-version"] = "2014-05-01";
        }

        internal List<LocationInformation> Locations { get; set; }

        protected override void ResponseCallback(HttpWebResponse webResponse)
        {
            Locations = Parse(webResponse, BaseParser.GetSubscriberLocationsParser);
            SitAndWait.Set();
        }
    }
}