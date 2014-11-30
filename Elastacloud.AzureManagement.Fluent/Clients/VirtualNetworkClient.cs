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
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using System.Xml.Linq;
using Elastacloud.AzureManagement.Fluent.Clients.Interfaces;
using Elastacloud.AzureManagement.Fluent.Commands.VirtualNetworks;
using Elastacloud.AzureManagement.Fluent.Helpers;
using Elastacloud.AzureManagement.Fluent.Types.Exceptions;
using Elastacloud.AzureManagement.Fluent.Types.VirtualNetworks;
using Elastacloud.AzureManagement.Fluent.VirtualNetwork;

namespace Elastacloud.AzureManagement.Fluent.Clients
{
    /// <summary>
    /// Creates a virtual networking client which has the capability to return information about the vnet
    /// and add things to it
    /// </summary>
    public class VirtualNetworkClient : IVirtualNetworkingClient
    {
        public VirtualNetworkClient(string subscriptionId, X509Certificate2 managementCertificate)
        {
            SubscriptionId = subscriptionId;
            ManagementCertificate = managementCertificate;
        }

        public X509Certificate2 ManagementCertificate { get; set; }

        public string SubscriptionId { get; set; }
        /// <summary>
        /// Gets the available virtual networks in the correct order binding the address ranges to the subnets
        /// </summary>
        public IEnumerable<VirtualNetworkingUtils.VirtualNetwork> GetAvailableVirtualNetworks()
        {
            var command = new ListVirtualNetworksCommand()
            {
                SubscriptionId = SubscriptionId,
                Certificate = ManagementCertificate
            };
            command.Execute();
            return VirtualNetworkingUtils.ConvertVNetToHierarchicalModel(command.VirtualNetworks);
        }

        /// <summary>
        /// Creates an empty VNET to add address ranges and subnets 
        /// </summary>
        public string AddSubnetToAddressRange(string networkName, string addressRange, string subnetName)
        {
            var networkResponse = GetAvailableVirtualNetworks();
            var vNetSpecific = networkResponse.FirstOrDefault(vnet => vnet.Name == networkName);
            var subnetAddress = VirtualNetworkingUtils.NextAvailableSubnet(addressRange, vNetSpecific);
            if (subnetAddress == null)
            {
                throw new FluentManagementException("there is no space in the selected address range to create a new subnet", "VirtualNetworkClient");
            }
            var subnetTag = new SubnetTag()
            {
                NetworkName = networkName,
                AddressPrefix = addressRange,
                SubnetAddressRange = subnetAddress,
                SubnetName = subnetName
            };
            string xml = AddXmlSubnetToExistingNetworkingDefinition(subnetTag);
            var command = new SetVirtualNetworkConfigCommand(xml)
            {
                SubscriptionId = SubscriptionId,
                Certificate = ManagementCertificate
            };
            command.Execute();
            return xml;
        }

        /// <summary>
        /// Checks to see whether an IP address in a virtual network is available
        /// </summary>
        public AvailableIpAddresses IsIpAddressAvailable(string vnet, string ipToCheck)
        {
            var command = new GetAvailableIpAddressesCommand(vnet, ipToCheck)
            {
                SubscriptionId = SubscriptionId,
                Certificate = ManagementCertificate
            };
            command.Execute();
            return command.IpAddressCheck;
        }

        /// <summary>
        /// Gets all of the networking config for the entire set of vnets
        /// </summary>
        public string GetAllNetworkingConfig()
        {
            var command = new GetVirtualNetworkConfigCommand()
            {
                SubscriptionId = SubscriptionId,
                Certificate = ManagementCertificate
            };
            command.Execute();
            return command.VirtualNetworkConfig;
        }

        #region Network Details 
        public class SubnetTag
        {
            public string NetworkName { get; set; }
            public string AddressPrefix { get; set; }
            public string SubnetName { get; set; }
            public string SubnetAddressRange { get; set; }
        }

        public XNamespace Namespace = "http://schemas.microsoft.com/ServiceHosting/2011/07/NetworkConfiguration";

        private string AddXmlSubnetToExistingNetworkingDefinition(SubnetTag tag)
        {
            var xml = GetAllNetworkingConfig();
            var document = XDocument.Parse(xml);
            var virtualSites = document.Element(Namespace + "NetworkConfiguration")
                .Element(Namespace + "VirtualNetworkConfiguration")
                .Element(Namespace + "VirtualNetworkSites")
                .Elements(Namespace + "VirtualNetworkSite")
                .FirstOrDefault(site => site.Attributes("name").FirstOrDefault().Value == tag.NetworkName);
            var addedSubnetTag = new XElement(Namespace + "Subnet",
                new XAttribute("name", tag.SubnetName),
                new XElement(Namespace + "AddressPrefix", tag.SubnetAddressRange)
                );
            virtualSites.Element(Namespace + "Subnets").Add(addedSubnetTag);
            return document.ToStringFullXmlDeclaration();
        }

        #endregion
    }
}
