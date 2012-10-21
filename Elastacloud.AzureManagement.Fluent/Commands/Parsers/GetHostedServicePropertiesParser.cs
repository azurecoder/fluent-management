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
    internal class GetHostedServicePropertiesParser : BaseParser
    {
        public GetHostedServicePropertiesParser(XDocument response) : base(response)
        {
            CommandResponse = new List<Deployment>();
        }

        internal override void Parse()
        {
            IEnumerable<XElement> rootElements = Document.Element(GetSchema() + RootElement)
                .Elements(GetSchema() + "Deployments");
            if (rootElements == null)
                return;

            rootElements = rootElements.Elements(GetSchema() + "Deployment");
            foreach (XElement deployment in rootElements)
            {
                    var objDeployment = new Deployment()
                        {
                            Name = deployment.Element(GetSchema() + "Name").Value,
                            Slot = (DeploymentSlot)Enum.Parse(typeof (DeploymentSlot), deployment.Element(GetSchema() + "DeploymentSlot").Value)
                        };
                    if (deployment.Elements(GetSchema() + "RoleInstanceList") != null)
                    {
                        var instanceListCount =
                            deployment.Elements(GetSchema() + "RoleInstanceList").Descendants().Count(a => a.Name == GetSchema() + "RoleInstance");
                        objDeployment.TotalRoleInstanceCount = instanceListCount;
                    }
                CommandResponse.Add(objDeployment);
            }
        }

        #region Overrides of BaseParser

        internal override string RootElement
        {
            get { return GetCloudServicePropertiesParser; }
        }

        protected override XNamespace GetSchema()
        {
            return XNamespace.Get(WindowsAzureSchema);
        }

        #endregion
    }
}