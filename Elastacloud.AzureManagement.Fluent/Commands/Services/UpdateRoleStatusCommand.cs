/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System;
using System.Text;
using System.Xml.Linq;
using Elastacloud.AzureManagement.Fluent.Helpers;
using Elastacloud.AzureManagement.Fluent.Types;

namespace Elastacloud.AzureManagement.Fluent.Commands.Services
{
    /// <summary>
    /// Used for updating the status of a role between running and suspended
    /// </summary>
    internal class UpdateRoleStatusCommand : ServiceCommand
    {
        /// <summary>
        /// Constructs a command to create an update to the status of a role
        /// </summary>
        internal UpdateRoleStatusCommand(string serviceName, DeploymentSlot slot, UpdateDeploymentStatus status)
        {
            OperationId = "hostedservices";
            ServiceType = "services";
            HttpCommand = serviceName + "/deploymentslots/" + slot.ToString().ToLower() + "/?comp=status";
            Status = status;
        }

        protected UpdateDeploymentStatus Status { get; set; }

        /// <summary>
        /// The deployment slot being used production|staging
        /// </summary>
        internal DeploymentSlot DeploymentSlot { get; set; }

        
        //https://management.core.windows.net/<subscription-id>/services/hostedservices/<service-name>/deploymentslots/<deployment-slot-name>/?comp=status
        /// <summary>
        /// The Xml payload that is created and sent to the Fabric with the create deployment parameters
        /// </summary>
        /// <returns>A string Xml document representation</returns>
        protected override string CreatePayload()
        {
            XNamespace ns = "http://schemas.microsoft.com/windowsazure";
            var doc = new XDocument(
                new XDeclaration("1.0", "utf-8", ""),
                new XElement(ns + "UpdateDeploymentStatus", new XElement(ns + "Status", Status)));
            return doc.ToStringFullXmlDeclaration();
        }
    }
}