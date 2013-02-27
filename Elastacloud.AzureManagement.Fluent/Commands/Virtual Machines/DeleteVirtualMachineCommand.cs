/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System;
using Elastacloud.AzureManagement.Fluent.Commands.Services;
using Elastacloud.AzureManagement.Fluent.VirtualMachines.Classes;

namespace Elastacloud.AzureManagement.Fluent.Commands.VirtualMachines
{
    /// <summary>
    /// Creates a deployment for a virtual machine and allows some preconfigured defaults from the image gallery 
    /// </summary>
    internal class DeleteVirtualMachineCommand : ServiceCommand
    {
        // DELETE https://management.core.windows.net/<subscription-id>/services/hostedservices/<service-name>/deployments/<deployment-name>/roles/<role-name>
        /// <summary>
        /// Used to construct the command to create a virtual machine deployment including the creation of a role
        /// </summary>
        internal DeleteVirtualMachineCommand(WindowsVirtualMachineProperties properties)
        {
            AdditionalHeaders["x-ms-version"] = "2012-03-01";
            OperationId = "hostedservices";
            ServiceType = "services";
            HttpCommand = String.Format("{0}/deployments/{1}/roles/{2}", properties.CloudServiceName, properties.DeploymentName, properties.RoleName);
            HttpVerb = HttpVerbDelete;
            Properties = properties;
        }

        /// <summary>
        /// The full virtual machine properties of the windows instance the needs to be deployed
        /// </summary>
        public WindowsVirtualMachineProperties Properties { get; set; }

        /// <summary>
        /// returns the name of the command
        /// </summary>
        public override string ToString()
        {
            return "DeleteVirtualMachineCommand";
        }
    }
}