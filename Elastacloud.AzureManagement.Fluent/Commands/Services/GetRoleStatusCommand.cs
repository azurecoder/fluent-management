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
    /// This commands checks the status of a particular role - whether it's running, stopped etc.
    /// </summary>
    internal class GetRoleStatusCommand : ServiceCommand
    {
        /// <summary>
        /// Used to create an instance of GetRoleStatusCommand
        /// </summary>
        //https://management.core.windows.net/<subscription-id>/services/hostedservices/<service-name>/deploymentslots/<deployment-slot>
        internal GetRoleStatusCommand(string hostedServiceName, string roleName, DeploymentSlot slot)
        {
            OperationId = "hostedservices";
            ServiceType = "services";
            HttpCommand = (HostedServiceName = hostedServiceName) + "/deploymentslots/" + slot.ToString().ToLower();
            ;
            HttpVerb = HttpVerbGet;
            RoleName = roleName;
            Slot = slot;
        }

        /// <summary>
        /// Shows the status of the particular requested role
        /// </summary>
        public RoleStatus RoleStatus { get; internal set; }

        /// <summary>
        /// The DeploymentSlot that is being requested
        /// </summary>
        internal DeploymentSlot Slot { get; set; }

        /// <summary>
        /// the name of the role being watched
        /// </summary>
        internal string RoleName { get; set; }

        /// <summary>
        /// The name of the hosted service we're interested in
        /// </summary>
        internal string HostedServiceName { get; set; }

        /// <summary>
        /// The response callback used to parse the Xml response 
        /// </summary>
        /// <param name="webResponse">The web response</param>
        protected override void ResponseCallback(HttpWebResponse webResponse)
        {
            RoleStatus = Parse(webResponse, BaseParser.GetRoleStatusParser, new GetRoleStatusParser(null));
            SitAndWait.Set();
        }
    }
}