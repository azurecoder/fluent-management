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
    internal class GetDeploymenRoleNamesCommand : ServiceCommand
    {
        /// <summary>
        /// Constructs a GetHostedServiceList command
        /// </summary>
        // https://management.core.windows.net/<subscription-id>/services/hostedservices/<service-name>/deploymentslots/<deployment-slot>
        internal GetDeploymenRoleNamesCommand(string serviceName, DeploymentSlot slot = DeploymentSlot.Production)
        {
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
        /// The list of rolenames present in the dpeloyment
        /// </summary>
        public List<string> RoleNames { get; set; } 

        /// <summary>
        /// The response with already parsed xml
        /// </summary>
        /// <param name="webResponse">The HttpWebResponse</param>
        protected override void ResponseCallback(HttpWebResponse webResponse)
        {
            RoleNames = Parse(webResponse, BaseParser.GetDeploymentRoleNamesParser, new GetDeploymentRoleNamesParser(null));
            SitAndWait.Set();
        }
    }
}