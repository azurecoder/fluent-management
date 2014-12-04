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
    internal class ListStorageAccountsParser : BaseParser
    {
        public ListStorageAccountsParser(XDocument response)
            : base(response)
        {
            CommandResponse = new List<StorageAccount>();
        }

        internal override void Parse()
        {
            IEnumerable<XElement> rootElements = Document.Element(GetSchema() + "StorageServices")
                .Elements(GetSchema() + "StorageService");
            foreach (XElement hostedService in rootElements)
            {
                var service = new StorageAccount
                                  {
                                      Name = (string) hostedService.Element(GetSchema() + "ServiceName"),
                                      Url = (string) hostedService.Element(GetSchema() + "Url"),
                                      Location = (string) hostedService.Element(GetSchema() + "StorageServiceProperties")
                                                .Element(GetSchema() + "Location")
                                  };
                // this can happen if there is an affinity group - then there is no location -
                // we get a primary geolocation though
                if (service.Location == null)
                {
                    service.Location = (string) hostedService.Element(GetSchema() + "StorageServiceProperties")
                        .Element(GetSchema() + "GeoPrimaryRegion");
                }
                CommandResponse.Add(service);
            }
        }

        #region Overrides of BaseParser

        internal override string RootElement
        {
            get { return ListStorageAccountsParser; }
        }

        protected override XNamespace GetSchema()
        {
            return XNamespace.Get(WindowsAzureSchema);
        }

        #endregion
    }
}