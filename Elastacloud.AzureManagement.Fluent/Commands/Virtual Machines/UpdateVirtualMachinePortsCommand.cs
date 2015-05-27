/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System;
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
    internal class UpdateVirtualMachinePortsCommand : ServiceCommand
    {
        /// <summary>
        /// Used to construct the command to create a virtual machine deployment including the creation of a role
        /// </summary>
        /// https://management.core.windows.net/<subscription-id>/services/hostedservices/<cloudservice-name>/deployments/<deployment-name>/roles/<role-name>
        internal UpdateVirtualMachinePortsCommand(string cloudServiceName, string vmName, params InputEndpoint[] ports)
        {
            AdditionalHeaders["x-ms-version"] = "2012-03-01";
            OperationId = "hostedservices";
            ServiceType = "services";
            HttpVerb = HttpVerbPut;
            HttpCommand = String.Format("{0}/deployments/{0}/roles/{1}", (CloudServiceName = cloudServiceName), vmName);
            AllEndpoints = new InputEndpoints();
            foreach (var port in ports)
            {
                AllEndpoints.AddEndpoint(port);
            }
        }

        public InputEndpoints AllEndpoints { private set; get; }
        /// <summary>
        /// The name of the cloud service to which the virtual machine will be created 
        /// </summary>
        public string CloudServiceName { get; set; }
        /// <summary>
        /// Specifies the name of the role in the cloud service
        /// </summary>
        public string RoleName { get; set; }

        
        /// <summary>
        /// Creates a deployment payload for a predefined template 
        /// </summary>
        /// <returns>A string xml representation</returns>
        protected override string CreatePayload()
        {
            var deployment = new NetworkConfigurationSet {InputEndpoints = AllEndpoints};
            var template = new PersistentVMRole {NetworkConfigurationSet = deployment, RoleName = RoleName};
            var document = new XDocument(template.GetXmlTree());
            return document.ToStringFullXmlDeclarationWithReplace();
        }

        /// <summary>
        /// returns the name of the command
        /// </summary>
        public override string ToString()
        {
            return "UpdateVirtualMachinePortsCommand";
        }
    }
}