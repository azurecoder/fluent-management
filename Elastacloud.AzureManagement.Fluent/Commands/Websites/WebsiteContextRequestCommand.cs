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
    internal class WebsiteContextRequestCommand : ServiceCommand
    {
        /// <summary>
        /// Constructs a websites list command
        /// </summary>
        internal WebsiteContextRequestCommand(string region)
        {
            HttpVerb = HttpVerbGet;
            ServiceType = "services";
            OperationId = "webspaces";
            // keep this in to ensure no 403
            HttpCommand = String.Format("{0}/sites/", region);
            /* propertiesToInclude=repositoryuri%2Cpublishingpassword%2Cpublishingusername */
        }
        /// <summary>
        /// Contains the names that will be used in further request to list the websites in regions 
        /// </summary>
        public List<Website> Websites { get; set; }

        /// <summary>
        /// Used to populate the website regions
        /// </summary>
        protected override void ResponseCallback(HttpWebResponse webResponse)
        {
            Websites = Parse(webResponse, BaseParser.WebsiteListParser, new WebsiteListParser(null));
            SitAndWait.Set();
        }
        /// <summary>
        /// Check here for a 404 error and suppress
        /// </summary>
        /// <param name="exception"></param>
        protected override void ErrorResponseCallback(WebException exception)
        {
            if (exception.Status == WebExceptionStatus.ProtocolError)
                return;
            throw exception;
        }
    }
}