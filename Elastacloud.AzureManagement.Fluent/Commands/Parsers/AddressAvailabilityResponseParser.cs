/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using Elastacloud.AzureManagement.Fluent.Types.VirtualNetworks;

namespace Elastacloud.AzureManagement.Fluent.Commands.Parsers
{
    /// <summary>
    /// Parses the response of a list of disks within a subscription
    /// </summary>
    internal class AddressAvailabilityResponseParser : BaseParser
    {
        public AddressAvailabilityResponseParser(XDocument response)
            : base(response)
        {
            CommandResponse = new AvailableIpAddresses();
        }

        internal override void Parse()
        {
            /*
             * <?xml version="1.0" encoding="utf-8"?>
<AddressAvailabilityResponse xmlns=”http://schemas.microsoft.com/windowsazure”>
  <IsAvailable>indicator-of-availability</IsAvailable>
  <AvailableAddresses>
    <AvailableAddress>ip-address-1</AvailableAddress>
    <AvailableAddress>ip-address-2</AvailableAddress>
    <AvailableAddress>ip-address-3</AvailableAddress>
    <AvailableAddress>ip-address-4</AvailableAddress>
    <AvailableAddress>ip-address-5</AvailableAddress>
  </AvailableAddresses>
</AddressAvailabilityResponse>*/
            var availability = new AvailableIpAddresses {AvailableIps = new List<IPAddress>()};
            var rootElement = Document.Element(GetSchema() + "AddressAvailabilityResponse");
            if (rootElement.Element(GetSchema() + "IsAvailable") != null)
            {
                availability.IpIsAvailable = bool.Parse(rootElement.Element(GetSchema() + "IsAvailable").Value);
            }
            if (!availability.IpIsAvailable)
            {
                foreach (XElement address in rootElement.Element(GetSchema() + "AvailableAddresses").Elements(GetSchema() + "AvailableAddress"))
                {
                    availability.AvailableIps.Add(IPAddress.Parse(address.Value));
                }
            }

            CommandResponse = availability;
        }

        #region Overrides of BaseParser

        internal override string RootElement
        {
            get { return "AddressAvailabilityResponse"; }
        }

        protected override XNamespace GetSchema()
        {
            return XNamespace.Get(WindowsAzureSchema);
        }

        #endregion
    }
}