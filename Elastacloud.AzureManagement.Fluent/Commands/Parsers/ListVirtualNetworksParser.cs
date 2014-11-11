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
using System.Xml.XPath;
using Elastacloud.AzureManagement.Fluent.Types;
using Elastacloud.AzureManagement.Fluent.Types.VirtualMachines;
using Elastacloud.AzureManagement.Fluent.Types.VirtualNetworks;

namespace Elastacloud.AzureManagement.Fluent.Commands.Parsers
{
    /// <summary>
    /// Parses the response of a list of disks within a subscription
    /// </summary>
    internal class ListVirtualNetworksParser : BaseParser
    {
        public ListVirtualNetworksParser(XDocument response)
            : base(response)
        {
            CommandResponse = new List<VirtualNetworkSite>();
        }

        internal override void Parse()
        {
            /*
             * <VirtualNetworkSites xmlns:i="http://www.w3.org/2001/XMLSchema-instance" xmlns="http://schemas.microsoft.com/windowsazure">
  <VirtualNetworkSite>
    <Name>name-of-virtual-network-site</Name>
    <Label>label-of-virtual-network-site</Label>
    <Id>identifier-of-virtual-network-site</Id>
    <AffinityGroup>name-of-affinity-group</AffinityGroup>
    <Location>location-of-virtual-network-site</Location>
    <State>state-of-virtual-network-site</State>
    <AddressSpace>
      <AddressPrefixes>
        <AddressPrefix>CIDR-identifier</AddressPrefix>
      </AddressPrefixes>
    </AddressSpace>
    <Subnets>
      <Subnet>
        <Name>subnet-name</Name>
        <AddressPrefix>CIDR-identifier</AddressPrefix>
      </Subnet>
    </Subnets>
    <DnsServers>
      <DnsServer>
        <Name>primary-DNS-name</Name>
        <Address>IPV4-address-of-the-DNS-server</Address>
      </DnsServer>
    <Gateway>
      <Profile>gateway-profile-size</Profile>
      <Sites>
        <LocalNetworkSite>
          <Name>local-site-name</Name>
          <AddressSpace>
            <AddressPrefixes>
              <AddressPrefix>CIDR-identifier</AddressPrefix>
            </AddressPrefixes>
          </AddressSpace>
          <VpnGatewayAddress>IPV4-address-of-the-vpn-gateway</VpnGatewayAddress>
          <Connections>
            <Connection>
              <Type>connection-type</Type>
            </Connection>
          </Connections>
        </LocalNetworkSite>
      </Sites>
      <VPNClientAddressPool>
        <AddressPrefixes>
          <AddressPrefix>CIDR-identifier</AddressPrefix>
        </AddressPrefixes>
      </VPNClientAddressPool>
    </Gateway>    
  </VirtualNetworkSite>
</VirtualNetworkSites>*/
            IEnumerable<XElement> rootElements = Document.Element(GetSchema() + "VirtualNetworkSites")
                .Elements(GetSchema() + "VirtualNetworkSite");
            foreach (XElement virtualSite in rootElements)
            {
                var site = new VirtualNetworkSite();
                // get the affinity group if it exists
                if (virtualSite.Element(GetSchema() + "Name") != null)
                {
                    site.Name = virtualSite.Element(GetSchema() + "Name").Value;
                }
                if (virtualSite.Element(GetSchema() + "Label") != null)
                {
                    site.Label = virtualSite.Element(GetSchema() + "Label").Value;
                }
                if (virtualSite.Element(GetSchema() + "Id") != null)
                {
                    site.Location = virtualSite.Element(GetSchema() + "Id").Value;
                }
                if (virtualSite.Element(GetSchema() + "Location") != null)
                {
                    site.Location = virtualSite.Element(GetSchema() + "Location").Value;
                }
                if (virtualSite.Element(GetSchema() + "State") != null)
                {
                    site.State = virtualSite.Element(GetSchema() + "State").Value;
                }
                site.Subnets = new List<Subnet>();
                if (virtualSite.Element(GetSchema() + "Subnets") != null)
                {
                    foreach (var subNet in virtualSite.Element(GetSchema() + "Subnets").Elements(GetSchema() + "Subnet").Select(subnet => new Subnet()
                    {
                        Name = (string) subnet.Element(GetSchema() + "Name"),
                        CidrAddressRange = (string) subnet.Element(GetSchema() + "AddressPrefix")
                    }))
                    {
                        site.Subnets.Add(subNet);
                    }
                }
                
                CommandResponse.Add(site);
            }
        }

        #region Overrides of BaseParser

        internal override string RootElement
        {
            get { return "VirtualNetworkSites"; }
        }

        protected override XNamespace GetSchema()
        {
            return XNamespace.Get(WindowsAzureSchema);
        }

        #endregion
    }
}