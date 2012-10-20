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
using Elastacloud.AzureManagement.Fluent.Types;

namespace Elastacloud.AzureManagement.Fluent.Commands.Services
{
    /// <summary>
    /// This commands checks all of the running role instance and returns back a boolean if all of the role instances are running
    /// </summary>
    internal class GetAggregateDeploymentStatusCommand : ServiceCommand
    {
        public bool AllDeploymentNodesRunning;

        /// <summary>
        /// Used to create an instance of GetAggregateDeploymentStatusCommand
        /// </summary>
        //https://management.core.windows.net/<subscription-id>/services/hostedservices/<service-name>/deploymentslots/<deployment-slot>
        internal GetAggregateDeploymentStatusCommand(string hostedServiceName, DeploymentSlot slot)
        {
            OperationId = "hostedservices";
            ServiceType = "services";
            HttpCommand = (HostedServiceName = hostedServiceName) + "/deploymentslots/" + slot.ToString().ToLower();
            HttpVerb = HttpVerbGet;
        }

        internal string HostedServiceName { get; set; }

        protected override void ResponseCallback(HttpWebResponse webResponse)
        {
            AllDeploymentNodesRunning = Parse(webResponse, BaseParser.GetAggregateDeploymentsParser,
                                              new GetAggregateDeploymentStatusParser(null));
            SitAndWait.Set();
        }
    }
}