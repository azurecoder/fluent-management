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
    internal class GetHostedServiceListParser : BaseParser
    {
        public GetHostedServiceListParser(XDocument response)
            : base(response)
        {
            CommandResponse = new List<HostedService>();
        }

        internal override void Parse()
        {
            IEnumerable<XElement> rootElements = Document.Element(GetSchema() + RootElement)
                .Elements(GetSchema() + "HostedService");
            foreach (XElement hostedService in rootElements)
            {
                var service = new HostedService
                                  {
                                      Name = (string) hostedService.Element(GetSchema() + "ServiceName"),
                                      Url = (string) hostedService.Element(GetSchema() + "Url")
                                  };
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