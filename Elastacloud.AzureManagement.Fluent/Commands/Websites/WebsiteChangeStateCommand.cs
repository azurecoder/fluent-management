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
    internal class WebsiteChangeStateCommand : ServiceCommand
    {
        /// <summary>
        /// Constructs a websites list command
        /// </summary>
        internal WebsiteChangeStateCommand(Website site, WebsiteState state)
        {
            HttpVerb = HttpVerbPut;
            ServiceType = "services";
            OperationId = "webspaces";
            // keep this in to ensure no 403
            HttpCommand = String.Format("{0}/sites/{1}", site.Webspace, site.Name);
            Site = site;
            State = state;
        }

        protected override string CreatePayload()
        {
            XNamespace ns = "http://schemas.microsoft.com/windowsazure";
            XNamespace array = "http://schemas.microsoft.com/2003/10/Serialization/Arrays";
            var doc = new XDocument(
                    new XDeclaration("1.0", "utf-8", ""),
                    new XElement(ns + "Site",
                                             new XElement(ns + "HostNames", 
                                                 new XElement(array + "string", Site.Name + "azurewebsites.net")),
                                             new XElement(ns + "Name", Name),
                                             new XElement(ns + "State", State.ToString())));
            return doc.ToStringFullXmlDeclaration();
        }

        /// <summary>
        /// The website being used to change state
        /// </summary>
        public Website Site { get; set; }
        /// <summary>
        /// The state of the website Running or Stopped
        /// </summary>
        public WebsiteState State { get; set; }
       
    }
}