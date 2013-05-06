/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System;
using System.Xml.Linq;
using Elastacloud.AzureManagement.Fluent.Commands.Services;
using Elastacloud.AzureManagement.Fluent.Helpers;
using Elastacloud.AzureManagement.Fluent.Types.Websites;

namespace Elastacloud.AzureManagement.Fluent.Commands.Websites
{
    /// <summary>
    /// Used to create a hosted service within a given subscription
    /// </summary>
    internal class CreateWebsiteRepositoryCommand : ServiceCommand
    {
        /// <summary>
        /// Constructs a websites list command
        /// </summary>
        internal CreateWebsiteRepositoryCommand(Website website, string uri = null)
        {
            HttpVerb = HttpVerbGet;
            ServiceType = "services";
            OperationId = "webspaces";
            Website = website;
            HttpVerb = HttpVerbPost;
            // keep this in to ensure no 403
            HttpCommand = String.Format("{0}/sites/{1}/repository", Website.Webspace, Website.Name);
            /* propertiesToInclude=repositoryuri%2Cpublishingpassword%2Cpublishingusername */
        }
        /// <summary>
        /// The website and all of itsconfig and properties
        /// </summary>
        protected Website Website { get; set; }
        /// <summary>
        /// The uri needed to overload for a remote repository
        /// </summary>
        public string Uri { get; set; }

        protected override string CreatePayload()
        {
            XNamespace xmlns = "http://schemas.microsoft.com/2003/10/Serialization/Arrays";

            if (!String.IsNullOrEmpty(Uri))
            {
                var doc = new XDocument(
                    new XDeclaration("1.0", "utf-8", ""),
                    new XElement(xmlns + "anyURI", Uri));
                return doc.ToStringFullXmlDeclaration();
            }

            return base.CreatePayload();
        }
    }
}