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
using System.Xml.Linq;
using Elastacloud.AzureManagement.Fluent.Types;

namespace Elastacloud.AzureManagement.Fluent.Commands.Parsers
{
    /// <summary>
    /// Gets a list of roles for a deployment that can be summed 
    /// </summary>
    internal class GetDeploymentRoleNamesParser : BaseParser
    {
        /// <summary>
        /// Creates a new GetRoleStatusParser to pull the status of the deployment
        /// </summary>
        /// <param name="response">The document returned by the web</param>
        public GetDeploymentRoleNamesParser(XDocument response)
            : base(response)
        {
            CommandResponse = new List<string>();
        }

        /// <summary>
        /// Parses the response to give a boolean value which lets us know whether all of the nodes are in the running state (true) or (false) 
        /// even a single one isn't
        /// </summary>
        internal override void Parse()
        {
            // have to ensure that both the IaaS and PaaS roles are returned
            var names = new List<string>();
            var roles = Document.Descendants(GetSchema() + "Role").Elements(GetSchema() + "RoleName");
            var vmRoles = Document.Descendants(GetSchema() + "PersistentVMRole").Descendants(GetSchema() + "RoleName");
            names = AddRoleNamesToCollection(roles, names);
            names = AddRoleNamesToCollection(vmRoles, names);

            CommandResponse = names;
        }

        /// <summary>
        /// Returns all of the names of roles for a given collection of elements
        /// </summary>
        /// <param name="elements">The element collection</param>
        /// <param name="names">The string list of role names</param>
        /// <returns>The updated names collection</returns>
        private List<string> AddRoleNamesToCollection(IEnumerable<XElement> elements, List<string> names)
        {
            names.AddRange(elements.Select(xElement => xElement.Value));
            return names;
        }

        #region Overrides of BaseParser

        /// <summary>
        /// The GetRoleStatus parser root element
        /// </summary>
        internal override string RootElement
        {
            get { return GetDeploymentRoleNamesParser; }
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