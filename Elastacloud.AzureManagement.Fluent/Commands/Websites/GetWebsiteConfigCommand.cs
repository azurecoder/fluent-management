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
    internal class GetWebsiteConfigCommand : ServiceCommand
    {
        public const string WebsitePostfix = ".azurewebsites.net";
        /// <summary>
        /// Constructs a websites list command
        /// </summary>
        internal GetWebsiteConfigCommand(Website website)
        {
            HttpVerb = HttpVerbGet;
            ServiceType = "services";
            OperationId = "webspaces";
            HttpVerb = HttpVerbGet;
            // keep this in to ensure no 403
            HttpCommand = String.Format("{0}/sites/{1}/config", website.Webspace, website.Name);
            /* propertiesToInclude=repositoryuri%2Cpublishingpassword%2Cpublishingusername */
        }

        public WebsiteConfig Config { get; set; }

        protected override void ResponseCallback(HttpWebResponse webResponse)
        {
            Config = Parse(webResponse, BaseParser.WebsiteConfigParser, new WebsiteConfigParser(null));
            SitAndWait.Set();
        }
        
    }
}