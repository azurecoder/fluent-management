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
using Elastacloud.AzureManagement.Fluent.Types;
using Elastacloud.AzureManagement.Fluent.Types.Exceptions;

namespace Elastacloud.AzureManagement.Fluent.Commands.Parsers
{
    /// <summary>
    /// Summary parses a response from the fabric for the roles in a deployment to give a status based on the health of the deployment
    /// </summary>
    internal class CheckCloudServiceNameAvailableParser : BaseParser
    {
        /// <summary>
        /// Creates a new GetRoleStatusParser to pull the status of the deployment
        /// </summary>
        /// <param name="response">The document returned by the web</param>
        public CheckCloudServiceNameAvailableParser(XDocument response)
            : base(response)
        {
            CommandResponse = false;
        }

        /// <summary>
        /// Parses the response to give a boolean value which lets us know whether all of the nodes are in the running state (true) or (false) 
        /// even a single one isn't
        /// </summary>
        internal override void Parse()
        {
            bool available = false;
            var availableResponse = (string) Document.Element(GetSchema() + RootElement)
                                      .Element(GetSchema() + "Result");
            bool success = bool.TryParse(availableResponse, out available);
            if (!success)
            {
                throw new FluentManagementException("unable to process cloud service name available response", "CheckCloudServiceNameAvailableParser");
            }

            CommandResponse = available;
        }

        #region Overrides of BaseParser

        /// <summary>
        /// The GetRoleStatus parser root element
        /// </summary>
        internal override string RootElement
        {
            get { return BaseParser.CloudServiceAvailable; }
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