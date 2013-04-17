/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

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
    internal class RestartVirtualMachineCommand : ServiceCommand
    {
        // https://management.core.windows.net/<subscription-id>/services/hostedservices/<service-name>/deployments/
        /// <summary>
        /// Used to construct the command to create a virtual machine deployment including the creation of a role
        /// </summary>
        internal RestartVirtualMachineCommand(WindowsVirtualMachineProperties properties)
        {
            AdditionalHeaders["x-ms-version"] = "2012-03-01";
            OperationId = "hostedservices";
            ServiceType = "services";
            //https://management.core.windows.net/<subscription-id>/services/hostedservices/<service-name>/deployments/<deployment-name>/roles/<role-name>/Operations
            HttpCommand = string.Format("{0}/deployments/{1}/roleinstances/{2}/Operations", properties.CloudServiceName, properties.DeploymentName, properties.RoleName);
            Properties = properties;
        }

        /// <summary>
        /// The full virtual machine properties of the windows instance the needs to be deployed
        /// </summary>
        public WindowsVirtualMachineProperties Properties { get; set; }

        /// <summary>
        /// Creates a deployment payload for a predefined template 
        /// </summary>
        /// <returns>A string xml representation</returns>
        /// POST https://management.core.windows.net/<subscription-id>/services/hostedservices/<service-name>/deployments/<deployment-name>/roleinstances/<role-name>/Operations
        protected override string CreatePayload()
        {
            /*<RestartRoleOperation xmlns="http://schemas.microsoft.com/windowsazure" xmlns:i="http://www.w3.org/2001/XMLSchema-instance">
   <OperationType>RestartRoleOperation</OperationType>
</RestartRoleOperation>*/
            XNamespace ns = "http://schemas.microsoft.com/windowsazure";
            var doc = new XDocument(
                new XDeclaration("1.0", "utf-8", ""),
                new XElement(ns + "RestartRoleOperation",
                             new XElement(ns + "OperationType", "RestartRoleOperation")));
            return doc.ToStringFullXmlDeclaration();
        }

        /// <summary>
        /// returns the name of the command
        /// </summary>
        public override string ToString()
        {
            return "RestartVirtualMachineCommand";
        }
    }
}