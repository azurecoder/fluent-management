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
    internal class GetHostedServiceListParser : BaseParser
    {
        public GetHostedServiceListParser(XDocument response)
            : base(response)
        {
            CommandResponse = new List<CloudService>();
        }

        internal override void Parse()
        {
            IEnumerable<XElement> rootElements = Document.Element(GetSchema() + RootElement)
                .Elements(GetSchema() + "HostedService");
            foreach (XElement hostedService in rootElements)
            {
                var service = new CloudService
                                  {
                                      Name = (string) hostedService.Element(GetSchema() + "ServiceName"),
                                      Url = (string) hostedService.Element(GetSchema() + "Url"),
                                      Created = DateTime.Parse(hostedService.Element(GetSchema() + "HostedServiceProperties").Element(GetSchema() + "DateCreated").Value),
                                      Modified = DateTime.Parse(hostedService.Element(GetSchema() + "HostedServiceProperties").Element(GetSchema() + "DateLastModified").Value),
                                      Status = (CloudServiceStatus)Enum.Parse(typeof(CloudServiceStatus), hostedService.Element(GetSchema() + "HostedServiceProperties").Element(GetSchema() + "Status").Value),
                                      Deployments = new List<Deployment>()
                                  };
                // if the cloud service has an affinity group then the location will not be returned 
                service.LocationOrAffinityGroup = (string)hostedService.Element(GetSchema() + "HostedServiceProperties").Element(GetSchema() + "Location") 
                    ?? (string)hostedService.Element(GetSchema() + "HostedServiceProperties").Element(GetSchema() + "AffinityGroup"); 
                
                CommandResponse.Add(service);
            }
        }

        #region Overrides of BaseParser

        internal override string RootElement
        {
            get { return GetHostedServiceListParser; }
        }

        protected override XNamespace GetSchema()
        {
            return XNamespace.Get(WindowsAzureSchema);
        }

        #endregion
    }
}