/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Xml.Linq;
using Elastacloud.AzureManagement.Fluent.Commands.Parsers;
using Elastacloud.AzureManagement.Fluent.Commands.Services;
using Elastacloud.AzureManagement.Fluent.Types;
using Elastacloud.AzureManagement.Fluent.Types.VirtualMachines;
using Elastacloud.AzureManagement.Fluent.VirtualMachines.Classes;

namespace Elastacloud.AzureManagement.Fluent.Commands.VirtualMachines
{
    /// <summary>
    /// gets a list of virtual machine disks within the subscription
    /// </summary>
    internal class GetVirtualDisksCommand : ServiceCommand
    {
        // GET https://management.core.windows.net/<subscription-id>/services/hostedservices/<service-name>/deploymentslots/Production
        /// <summary>
        /// Used to construct the command to create a virtual machine deployment including the creation of a role
        /// </summary>
        internal GetVirtualDisksCommand()
        {
            AdditionalHeaders["x-ms-version"] = "2012-03-01";
            OperationId = "disks";
            ServiceType = "services";
            HttpVerb = HttpVerbGet;
        }

        /// <summary>
        /// Used to bring back and parse the context response for a virtual machine
        /// </summary>
        /// <param name="webResponse">the Http web response</param>
        protected override void ResponseCallback(HttpWebResponse webResponse)
        {
            // get the cloud service deployments
            Disks = Parse(webResponse, BaseParser.GetVirtualDisksParser, new GetAllDisksParser(null));
            SitAndWait.Set();
        }

        /// <summary>
        /// The container vm for the reponse xml
        /// </summary>
        public List<VirtualDisk> Disks { get; set; }

        /// <summary>
        /// returns the name of the command
        /// </summary>
        public override string ToString()
        {
            return "GetVirtualDisksCommand";
        }
    }
}