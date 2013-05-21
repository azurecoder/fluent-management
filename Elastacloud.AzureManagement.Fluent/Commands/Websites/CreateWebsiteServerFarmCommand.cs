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
using System.Net;
using System.Text;
using System.Xml.Linq;
using Elastacloud.AzureManagement.Fluent.Commands.Parsers;
using Elastacloud.AzureManagement.Fluent.Commands.Services;
using Elastacloud.AzureManagement.Fluent.Helpers;
using Elastacloud.AzureManagement.Fluent.Types.Websites;

namespace Elastacloud.AzureManagement.Fluent.Commands.Websites
{
    /// <summary>
    /// Used to create a hosted service within a given subscription
    /// </summary>
    internal class CreateWebsiteServerFarmCommand : ServiceCommand
    {
        public const string WebsitePostfix = ".azurewebsites.net";
        /// <summary>
        /// Constructs a websites list command
        /// </summary>
        /// // https://management.core.windows.net/67b7c755-8382-4990-b612-0006cd24e1ba/services/webspaces/northeuropewebspace/serverfarms/DefaultServerFarm
        /// // <ServerFarm xmlns="http://schemas.microsoft.com/windowsazure" xmlns:i="http://www.w3.org/2001/XMLSchema-instance"><CurrentNumberOfWorkers>2</CurrentNumberOfWorkers><CurrentWorkerSize>Small</CurrentWorkerSize><Name>DefaultServerFarm</Name><NumberOfWorkers>2</NumberOfWorkers><Status>Ready</Status><WorkerSize>Small</WorkerSize></ServerFarm>
        internal CreateWebsiteServerFarmCommand(Website website)
        {
            HttpVerb = HttpVerbGet;
            ServiceType = "services";
            OperationId = "webspaces";
            Website = website;
            HttpVerb = HttpVerbPost;
            // keep this in to ensure no 403
            HttpCommand = String.Format("{0}/serverfarms/", Website.Webspace);
            /* propertiesToInclude=repositoryuri%2Cpublishingpassword%2Cpublishingusername */
        }

        protected Website Website { get; set; }

          /*<ServerFarm  xmlns="http://schemas.microsoft.com/windowsazure" xmlns:i="http://www.w3.org/2001/XMLSchema-instance">
             <Name>DefaultServerFarm</Name>
             <NumberOfWorkers>2</NumberOfWorkers>
             <WorkerSize>Small</WorkerSize>
            </ServerFarm>*/
        protected override string CreatePayload()
        {
            XNamespace xmlns = Namespaces.NsWindowsAzure;

            var doc = new XDocument(
                new XDeclaration("1.0", "utf-8", ""),
                new XElement(xmlns + "Name", Website.ServerFarm.Name),
                new XElement(xmlns + "NumberOfWorkers", Website.ServerFarm.InstanceCount),
                new XElement(xmlns + "WorkerSize", Website.ServerFarm.InstanceSize));

                return doc.ToStringFullXmlDeclaration();
        }
    }
}