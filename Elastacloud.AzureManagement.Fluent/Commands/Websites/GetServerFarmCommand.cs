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
    internal class GetServerFarmCommand : ServiceCommand
    {
        public const string WebsitePostfix = ".azurewebsites.net";
        /// <summary>
        /// Constructs a websites list command
        /// </summary>
        internal GetServerFarmCommand(Website website)
        {
            HttpVerb = HttpVerbGet;
            ServiceType = "services";
            OperationId = "webspaces";
            Website = website;
            HttpVerb = HttpVerbGet;
            // keep this in to ensure no 403
            HttpCommand = String.Format("{0}/serverfarms/{1}", Website.Webspace, Website.ServerFarm.Name);
            /* propertiesToInclude=repositoryuri%2Cpublishingpassword%2Cpublishingusername */
        }

        public Website Website { get; set; }

        // /// // <ServerFarm xmlns="http://schemas.microsoft.com/windowsazure" xmlns:i="http://www.w3.org/2001/XMLSchema-instance"><CurrentNumberOfWorkers>2</CurrentNumberOfWorkers><CurrentWorkerSize>Small</CurrentWorkerSize><Name>DefaultServerFarm</Name><NumberOfWorkers>2</NumberOfWorkers><Status>Ready</Status><WorkerSize>Small</WorkerSize></ServerFarm>
        protected override void ResponseCallback(HttpWebResponse webResponse)
        {
            Website.ServerFarm = Parse(webResponse, BaseParser.WebsiteServerFarm, new WebsiteServerFarmParser(null));
            SitAndWait.Set();
        }

    }
}