/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System;
using Elastacloud.AzureManagement.Fluent.Types;

namespace Elastacloud.AzureManagement.Fluent.Commands.Services
{
    /// <summary>
    /// Used to delete a deployment from a hosted service
    /// </summary>
    internal class DeleteCloudServiceAndDeploymentCommand : ServiceCommand
    {
        //https://management.core.windows.net/<subscription-id>/services/hostedservices/<cloudservice-name>/deployments/<deployment-name>
        /// <summary>
        /// Constructs a service deployment delete command with a given name and slot 
        /// </summary>
        /// <param name="serviceName">The name of the service which is being swept for deployments</param>
        /// <param name="deploymentName">The deployment slot used can be production or staging</param>
        internal DeleteCloudServiceAndDeploymentCommand(string serviceName, string deploymentName)
        {
            Name = serviceName;
            DeploymentName = deploymentName;
            HttpVerb = HttpVerbDelete;
            HttpCommand = String.Format("{0}/deployments/{1}?comp=media", Name, DeploymentName);
            ServiceType = "services";
            OperationId = "hostedservices";
            AdditionalHeaders["x-ms-version"] = "2013-08-01";
        }

        /// <summary>
        /// Production or Staging deployment slot
        /// </summary>
        internal string DeploymentName { get; set; }
    }
}