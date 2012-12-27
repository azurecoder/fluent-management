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
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Elastacloud.AzureManagement.Fluent.Helpers;
using Elastacloud.AzureManagement.Fluent.Types;

namespace Elastacloud.AzureManagement.Fluent.Commands.Parsers
{
    /// <summary>
    /// Gets a list of roles for a deployment that can be summed 
    /// </summary>
    internal class GetMobileServiceTablePermissionsParser : BaseParser
    {
        /// <summary>
        /// Creates a new GetRoleStatusParser to pull the status of the deployment
        /// </summary>
        /// <param name="response">The document returned by the web</param>
        public GetMobileServiceTablePermissionsParser(XDocument response)
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
            var dictionary = new Dictionary<string, string>(4);
            var serviceResource = Document.Element(GetSchema() + RootElement);
            dictionary["Insert"] = serviceResource.Element(GetSchema() + "Insert").Value;
            dictionary["Update"] = serviceResource.Element(GetSchema() + "Update").Value;
            dictionary["Read"] = serviceResource.Element(GetSchema() + "Read").Value;
            dictionary["Delete"] = serviceResource.Element(GetSchema() + "Delete").Value;
            CommandResponse = dictionary;
        }
       
        #region Overrides of BaseParser

        /// <summary>
        /// The GetRoleStatus parser root element
        /// </summary>
        internal override string RootElement
        {
            get { return GetMobileServiceTablePermissionsParser; }
        }

        /// <summary>
        /// Gets the schema of the Xml response
        /// </summary>
        protected override XNamespace GetSchema()
        {
            return Namespaces.NsWindowsAzureMobileServices;
        }

        #endregion
    }
}