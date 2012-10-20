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
using Elastacloud.AzureManagement.Fluent.Types;

namespace Elastacloud.AzureManagement.Fluent.Commands.Parsers
{
    /// <summary>
    /// Gets a list of roles for a deployment that can be summed 
    /// </summary>
    internal class GetDeploymentConfigurationParser : BaseParser
    {
        /// <summary>
        /// Creates a new GetRoleStatusParser to pull the status of the deployment
        /// </summary>
        /// <param name="response">The document returned by the web</param>
        public GetDeploymentConfigurationParser(XDocument response)
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
            var names = new List<string>();
            var configuration = Document.Element(GetSchema() + RootElement).Element(GetSchema() + "Configuration");
            // convert from UTF-8 base 64 - to bytes
            if (configuration == null)
                throw new ApplicationException("unable to proceed without a valid configuration");
            // convert from a base64 string to 
            byte[] configBytes = Convert.FromBase64String(configuration.Value);
            // use a UTF-8 encoder
            string xml = Encoding.UTF8.GetString(configBytes);
            // convert to an XDocument so that it can be loaded into a container
            var doc = XDocument.Parse(xml);
            CommandResponse = CscfgFile.GetAdHocInstance(doc);
        }
       
        #region Overrides of BaseParser

        /// <summary>
        /// The GetRoleStatus parser root element
        /// </summary>
        internal override string RootElement
        {
            get { return GetDeploymentConfigurationParser; }
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