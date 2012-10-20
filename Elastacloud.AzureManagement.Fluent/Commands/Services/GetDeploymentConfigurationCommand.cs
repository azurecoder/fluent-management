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
    internal class GetDeploymenConfigurationCommand : ServiceCommand
    {
        /// <summary>
        /// Constructs a GetHostedServiceList command
        /// </summary>
        // https://management.core.windows.net/<subscription-id>/services/hostedservices/<service-name>/deploymentslots/<deployment-slot>
        internal GetDeploymenConfigurationCommand(string serviceName, DeploymentSlot slot = DeploymentSlot.Production)
        {
            // need to increment the version in this request otherwise will not be able to check vm instances
            AdditionalHeaders["x-ms-version"] = "2012-03-01";
            OperationId = "hostedservices";
            ServiceType = "services";
            HttpCommand = (HostedServiceName = serviceName) + "/deploymentslots/" + slot.ToString().ToLower();
            HttpVerb = HttpVerbGet;
        }

        /// <summary>
        /// The name of the hosted service
        /// </summary>
        internal string HostedServiceName { get; set; }
        /// <summary>
        /// The DeploymentSlot being used by the application
        /// </summary>
        internal DeploymentSlot Slot { get; set; }
        /// <summary>
        /// The cscfg file that is returned to be parsed
        /// </summary>
        internal CscfgFile Configuration { get; set; } 

        /// <summary>
        /// The response with already parsed xml
        /// </summary>
        /// <param name="webResponse">The HttpWebResponse</param>
        protected override void ResponseCallback(HttpWebResponse webResponse)
        {
            Configuration = Parse(webResponse, BaseParser.GetDeploymentConfigurationParser, new GetDeploymentConfigurationParser(null));
            SitAndWait.Set();
        }
    }
}