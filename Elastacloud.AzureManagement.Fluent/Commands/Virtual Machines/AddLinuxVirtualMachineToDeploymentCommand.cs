/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System.Collections.Generic;
using System.Xml.Linq;
using Elastacloud.AzureManagement.Fluent.Commands.Services;
using Elastacloud.AzureManagement.Fluent.Helpers;
using Elastacloud.AzureManagement.Fluent.Types;
using Elastacloud.AzureManagement.Fluent.Types.VirtualMachines;
using Elastacloud.AzureManagement.Fluent.VirtualMachines.Classes;
using Deployment = Elastacloud.AzureManagement.Fluent.Types.VirtualMachines.Deployment;

namespace Elastacloud.AzureManagement.Fluent.Commands.VirtualMachines
{
    /// <summary>
    /// Creates a deployment for a virtual machine and allows some preconfigured defaults from the image gallery 
    /// </summary>
    internal class AddLinuxVirtualMachineToDeploymentCommand : ServiceCommand
    {
        // https://management.core.windows.net/<subscription-id>/services/hostedservices/<service-name>/deployments/
        /// <summary>
        /// Used to construct the command to create a virtual machine deployment including the creation of a role
        /// </summary>
        // https://management.core.windows.net/<subscription-id>/services/hostedservices/<cloudservice-name>/deployments/<deploymentname>/roles
        internal AddLinuxVirtualMachineToDeploymentCommand(LinuxVirtualMachineProperties properties, string cloudServiceName)
        {
            AdditionalHeaders["x-ms-version"] = "2012-03-01";
            OperationId = "hostedservices";
            ServiceType = "services";
            Properties = properties;
            HttpCommand = (CloudServiceName = cloudServiceName) + "/deployments/" + properties.DeploymentName + "/roles";
        }

        /// <summary>
        /// The name of the cloud service to which the virtual machine will be created 
        /// </summary>
        public string CloudServiceName { get; set; }

        /// <summary>
        /// The full virtual machine properties of the windows instance the needs to be deployed
        /// </summary>
        public LinuxVirtualMachineProperties Properties { get; set; }

        /// <summary>
        /// Creates a deployment payload for a predefined template 
        /// </summary>
        /// <returns>A string xml representation</returns>
        protected override string CreatePayload()
        {
            var deployment = PersistentVMRole.AddAdhocLinuxRoleTemplates(new List<LinuxVirtualMachineProperties>(new[]{Properties}));
            var document = new XDocument(deployment[0].GetXmlTree());
            return document.ToStringFullXmlDeclarationWithReplace();
        }

        /// <summary>
        /// returns the name of the command
        /// </summary>
        public override string ToString()
        {
            return "AddLinuxVirtualMachineToDeploymentCommand";
        }
    }
}