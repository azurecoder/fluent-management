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
using System.Text;
using System.Xml.Linq;
using Elastacloud.AzureManagement.Fluent.Commands.Parsers;
using Elastacloud.AzureManagement.Fluent.Commands.Services;
using Elastacloud.AzureManagement.Fluent.Helpers;

namespace Elastacloud.AzureManagement.Fluent.Commands.Websites
{
    /// <summary>
    /// Used to create a hosted service within a given subscription
    /// </summary>
    internal class GetWebsiteListCommand : ServiceCommand
    {
        /// <summary>
        /// Constructs a websites list command
        /// </summary>
        internal GetWebsiteListCommand()
        {
            HttpVerb = HttpVerbGet;
            ServiceType = "services";
            OperationId = "webspaces";
            // keep this in to ensure no 403
            HttpCommand = "/";
        }
        /// <summary>
        /// Contains the names that will be used in further request to list the websites in regions 
        /// </summary>
        public List<string> WebsiteRegions { get; set; }

        /// <summary>
        /// Used to populate the website regions
        /// </summary>
        protected override void ResponseCallback(System.Net.HttpWebResponse webResponse)
        {
            WebsiteRegions = Parse(webResponse, BaseParser.WebsiteListParser, new WebsiteListParser(null));
            SitAndWait.Set();
        }
    }
}