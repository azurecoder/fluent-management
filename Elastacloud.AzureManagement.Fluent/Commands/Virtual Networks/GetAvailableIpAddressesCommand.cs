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
using System.Globalization;
using System.Net;
using System.Xml.Linq;
using Elastacloud.AzureManagement.Fluent.Commands.Parsers;
using Elastacloud.AzureManagement.Fluent.Commands.Services;
using Elastacloud.AzureManagement.Fluent.Helpers;
using Elastacloud.AzureManagement.Fluent.Types.VirtualMachines;
using Elastacloud.AzureManagement.Fluent.Types.VirtualNetworks;

namespace Elastacloud.AzureManagement.Fluent.Commands.VirtualMachines
{
    /// <summary>
    ///   Registers a virtual machine image for either Linux or Windowss     
    ///  </summary>
    public class GetAvailableIpAddressesCommand : ServiceCommand
    {
        // https://management.core.windows.net/<subscription-id>/services/networking/<virtual-network-name>
        /// <summary>
        ///   Lists all images that are registered in your subscriptions   
        ///  </summary>
        internal GetAvailableIpAddressesCommand(string vnetName, string ipAddress)
        {
            AdditionalHeaders["x-ms-version"] = "2014-02-01";
            OperationId = "networking";
            ServiceType = "services";
            IpAddress = ipAddress;
            HttpVerb = HttpVerbGet;
            HttpCommand = vnetName + "?op=checkavailability&address=" + ipAddress;
        }

        public string IpAddress { get; set; }

        /// <summary>
        /// The full virtual machine properties of the windows instance the needs to be deployed
        /// </summary>
        public AvailableIpAddresses IpAddressCheck { get; set; }

        /// <summary>
        /// Initially used via a response callback for commands which expect a async response 
        /// </summary>
        /// <param name="webResponse">the HttpWebResponse that will be sent back to the user from the request</param>
        protected override void ResponseCallback(HttpWebResponse webResponse)
        {
            // get the cloud service deployments
            IpAddressCheck = Parse(webResponse, "AddressAvailabilityResponse", new AddressAvailabilityResponseParser(null));
            IpAddressCheck.RequestedIp = IPAddress.Parse(IpAddress);
            SitAndWait.Set();
        }

        /// <summary>
        /// returns the name of the command
        /// </summary>
        public override string ToString()
        {
            return "GetAvailableIpAddressesCommand";
        }
    }
}
