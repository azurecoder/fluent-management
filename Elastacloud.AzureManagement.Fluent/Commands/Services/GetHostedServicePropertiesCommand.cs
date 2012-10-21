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
using Elastacloud.AzureManagement.Fluent.Types;

namespace Elastacloud.AzureManagement.Fluent.Commands.Services
{
    /// <summary>
    /// Used to return a list of hosted services to the client 
    /// </summary>
    internal class GetHostedServicePropertiesCommand : ServiceCommand
    {
        /// <summary>
        /// Constructs a GetHostedServiceList command
        /// </summary>
        internal GetHostedServicePropertiesCommand(string serviceName)
        {
            AdditionalHeaders["x-ms-version"] = "2012-03-01";
            OperationId = "hostedservices";
            ServiceType = "services";
            HttpVerb = HttpVerbGet;
            HttpCommand = serviceName + "?embed-detail=true";
        }

        /// <summary>
        /// A list of hosted services that live in the subscription
        /// </summary>
        internal List<Deployment> CloudServiceDeployments { get; set; }

        /// <summary>
        /// The response with already parsed xml
        /// </summary>
        /// <param name="webResponse">The HttpWebResponse</param>
        protected override void ResponseCallback(HttpWebResponse webResponse)
        {
            CloudServiceDeployments = Parse(webResponse, BaseParser.GetCloudServicePropertiesParser);
            SitAndWait.Set();
        }
    }
}