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
                        Slot = (DeploymentSlot) Enum.Parse(typeof (DeploymentSlot), deployment.Element(GetSchema() + "DeploymentSlot").Value),
                        DeploymentID = deployment.Element(GetSchema() + "PrivateID").Value,
                    };
                if (deployment.Elements(GetSchema() + "RoleInstanceList") != null)
                {
                    var instanceListCount =
                        deployment.Elements(GetSchema() + "RoleInstanceList").Descendants().Count(a => a.Name == GetSchema() + "RoleInstance");
                    objDeployment.TotalRoleInstanceCount = instanceListCount;
                    var instanceList = deployment.Elements(GetSchema() + "RoleInstanceList")
                        .Elements(GetSchema() + "RoleInstance")
                        .Select(xElement => new RoleInstance()
                        {
                            Name = (string) xElement.Element(GetSchema() + "InstanceName"), 
                            IpAddress = (string) xElement.Element(GetSchema() + "IpAddress"), 
                            Size = (VmSize) Enum.Parse(typeof (VmSize), (string) xElement.Element(GetSchema() + "InstanceSize")),
                            Status = (RoleInstanceStatus)Enum.Parse(typeof(RoleInstanceStatus), (string)xElement.Element(GetSchema() + "InstanceStatus")),
                            VirtualIpAddress = (string)xElement.Element(GetSchema() + "InstanceEndpoints").Elements(GetSchema() + "InstanceEndpoint").First()
                                            .Element(GetSchema() + "Vip")
                        }).ToList();
                    objDeployment.RoleInstances = instanceList;
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
