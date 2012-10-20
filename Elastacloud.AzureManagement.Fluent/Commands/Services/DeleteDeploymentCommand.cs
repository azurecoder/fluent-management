/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using Elastacloud.AzureManagement.Fluent.Types;

namespace Elastacloud.AzureManagement.Fluent.Commands.Services
{
    /// <summary>
    /// Used to delete a deployment from a hosted service
    /// </summary>
    internal class DeleteDeploymentCommand : ServiceCommand
    {
        // https://management.core.windows.net/<subscription-id>/services/hostedservices/<service-name>/deploymentslots/<deployment-slot>
        /// <summary>
        /// Constructs a service deployment delete command with a given name and slot 
        /// </summary>
        /// <param name="serviceName">The name of the service which is being swept for deployments</param>
        /// <param name="slot">The deployment slot used can be production or staging</param>
        internal DeleteDeploymentCommand(string serviceName, DeploymentSlot slot)
        {
            Name = serviceName;
            DeploymentSlot = slot;
            HttpVerb = HttpVerbDelete;
            HttpCommand = Name + "/deploymentslots/" + slot.ToString().ToLower();
            ServiceType = "services";
            OperationId = "hostedservices";
        }

        /// <summary>
        /// Production or Staging deployment slot
        /// </summary>
        internal DeploymentSlot DeploymentSlot { get; set; }
    }
}