/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System.Collections.Generic;
using System.Xml.Linq;
using Elastacloud.AzureManagement.Fluent.Types;

namespace Elastacloud.AzureManagement.Fluent.Commands.Parsers
{
    /// <summary>
    /// Gets a list of roles for a deployment that can be summed 
    /// </summary>
    internal class GetMobileServiceResourceParser : BaseParser
    {
        /// <summary>
        /// Creates a new GetRoleStatusParser to pull the status of the deployment
        /// </summary>
        /// <param name="response">The document returned by the web</param>
        public GetMobileServiceResourceParser(XDocument response)
            : base(response)
        {
            // set the commandresponse to a Cscfg file in the parse method
        }

        /// <summary>
        /// Parses the response to give a boolean value which lets us know whether all of the nodes are in the running state (true) or (false) 
        /// even a single one isn't
        /// </summary>
        internal override void Parse()
        {
            // have to ensure that both the IaaS and PaaS roles are returned
            var dictionary = new Dictionary<string, string>(6);
            dictionary["State"] = Document.Element(GetSchema() + RootElement).Element(GetSchema() + "State").Value;
            dictionary["Description"] = Document.Element(GetSchema() + RootElement).Element(GetSchema() + "Description").Value;
            var internalResources = Document.Element(GetSchema() + RootElement).Descendants(GetSchema() + "InternalResource");
            foreach (var internalResource in internalResources)
            {
                var type = internalResource.Element(GetSchema() + "Type").Value;
                switch (type)
                {
                    case Constants.MobileServicesSqlServerType:
                        var serverName = internalResource.Element(GetSchema() + "Name");
                        dictionary["ServerName"] = serverName != null ? serverName.Value : "Unprovisioned!";
                        dictionary["MobileServiceServerName"] = internalResource.Element(GetSchema() + "LogicalName").Value;
                        break;
                    case Constants.MobileServicesSqlDatabaseType:
                        var dbName = internalResource.Element(GetSchema() + "Name");
                        dictionary["DatabaseName"] = dbName != null ? dbName.Value : "Unprovisioned!";
                        dictionary["MobileServiceDatabaseName"] = internalResource.Element(GetSchema() + "LogicalName").Value;
                        break;
                }
            }
            CommandResponse = dictionary;
        }
       
        #region Overrides of BaseParser

        /// <summary>
        /// The GetRoleStatus parser root element
        /// </summary>
        internal override string RootElement
        {
            get { return GetMobileServiceResourcesParser; }
        }

        /// <summary>
        /// Gets the schema of the Xml response
        /// </summary>
        protected override XNamespace GetSchema()
        {
            return XNamespace.Get(WindowsAzureSchema);
        }

        #endregion
    }
}