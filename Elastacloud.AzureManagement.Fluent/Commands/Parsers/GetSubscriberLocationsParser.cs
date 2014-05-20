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
    internal class GetSubscriberLocationsParser : BaseParser
    {
        public GetSubscriberLocationsParser(XDocument response)
            : base(response)
        {
            CommandResponse = new List<LocationInformation>();
        }

        internal override void Parse()
        {
            IEnumerable<XElement> rootElements = Document.Element(GetSchema() + "Locations")
                .Elements(GetSchema() + "Location");
            foreach (XElement hostedService in rootElements)
            {
                var service = new LocationInformation
                {
                    Name = (string) hostedService.Element(GetSchema() + "Name"),
                    DisplayName = (string) hostedService.Element(GetSchema() + "DisplayName"),
                    VirtualMachineRolesSizes = new List<VmSize>(),
                    WebWorkerRolesSizes = new List<VmSize>()
                };
                foreach (var element in hostedService.Element(GetSchema() + "AvailableServices").Elements(GetSchema() + "AvailableService"))
                {
                    service.AvailableServices |=
                        (AvailableServices) Enum.Parse(typeof (AvailableServices), element.Value);
                }
                foreach (var element in hostedService.Element(GetSchema() + "ComputeCapabilities").Element(GetSchema() + "WebWorkerRoleSizes")
                   .Elements(GetSchema() + "RoleSize"))
                {
                    service.WebWorkerRolesSizes.Add((VmSize)Enum.Parse(typeof(VmSize), element.Value));
                }
                foreach (var element in hostedService.Element(GetSchema() + "ComputeCapabilities").Element(GetSchema() + "VirtualMachinesRoleSizes")
                   .Elements(GetSchema() + "RoleSize"))
                {
                    service.VirtualMachineRolesSizes.Add((VmSize)Enum.Parse(typeof(VmSize), element.Value));
                }
                CommandResponse.Add(service);
            }
        }

        #region Overrides of BaseParser

        internal override string RootElement
        {
            get { return GetSubscriberLocationsParser; }
        }

        protected override XNamespace GetSchema()
        {
            return XNamespace.Get(WindowsAzureSchema);
        }

        #endregion
    }
}