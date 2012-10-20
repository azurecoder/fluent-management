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
    /// used to determine whether a hosted service contains production deployments
    /// </summary>
    internal class GetHostedServiceContainsDeploymentCommand : ServiceCommand
    {
        /// <summary>
        /// Constructs a GetHostedServiceList command
        /// </summary>
        internal GetHostedServiceContainsDeploymentCommand(string serviceName, DeploymentSlot slot = DeploymentSlot.Production)
        {
            OperationId = "hostedservices";
            ServiceType = "services";
            HttpCommand = (HostedServiceName = serviceName) + "/deploymentslots/" + slot.ToString().ToLower();
            HttpVerb = HttpVerbGet;
        }

        /// <summary>
        /// A list of hosted services that live in the subscription
        /// </summary>
        internal string HostedServiceName { get; set; }
        /// <summary>
        /// A property to denote whether this contains a production deployment
        /// </summary>
        internal bool ContainsProductionDeployment { get; set; }

        /// <summary>
        /// The response with already parsed xml
        /// </summary>
        /// <param name="webResponse">The HttpWebResponse</param>
        protected override void ResponseCallback(HttpWebResponse webResponse)
        {
            ContainsProductionDeployment = true;
            SitAndWait.Set();
        }

        /// <summary>
        /// Will throw an exception if the response brings back a failure code
        /// </summary>
        /// <param name="exception">The WebException being sent to the class</param>
        protected override void ErrorResponseCallback(WebException exception)
        {
            if (exception.Status == WebExceptionStatus.ProtocolError)
            {
                ContainsProductionDeployment = false;
                SitAndWait.Set();
                return;
            }
            base.ErrorResponseCallback(exception);
        }
    }
}