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
using System.Xml.Linq;
using Elastacloud.AzureManagement.Fluent.Types;

namespace Elastacloud.AzureManagement.Fluent.Commands.Parsers
{
    /// <summary>
    /// Summary parses a response from the fabric for the nodes that are deployed and returns the requisite status for each role instance
    /// </summary>
    internal class GetAggregateDeploymentStatusParser : BaseParser
    {
        /// <summary>
        /// Creates a new aggregate deployment status reponse parser
        /// </summary>
        /// <param name="response"></param>
        public GetAggregateDeploymentStatusParser(XDocument response)
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
            IEnumerable<XElement> rootElements = Document.Element(GetSchema() + RootElement)
                .Descendants(GetSchema() + "RoleInstance");
            foreach (XElement roleInstance in rootElements)
            {
                string instanceStatus = roleInstance.Element(GetSchema() + "InstanceStatus").Value;
                var status = (DeploymentStatus) Enum.Parse(typeof (DeploymentStatus), instanceStatus);
                if (status != DeploymentStatus.ReadyRole)
                    return;
            }
            CommandResponse = true;
        }

        #region Overrides of BaseParser

        internal override string RootElement
        {
            get { return GetAggregateDeploymentsParser; }
        }

        protected override XNamespace GetSchema()
        {
            return XNamespace.Get(WindowsAzureSchema);
        }

        #endregion
    }
}