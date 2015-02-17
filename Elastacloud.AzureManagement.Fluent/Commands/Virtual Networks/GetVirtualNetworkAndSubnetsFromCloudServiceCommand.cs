/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using Elastacloud.AzureManagement.Fluent.Commands.Parsers;
using Elastacloud.AzureManagement.Fluent.Commands.Services;
using Elastacloud.AzureManagement.Fluent.Helpers;
using Elastacloud.AzureManagement.Fluent.Types.VirtualNetworks;

namespace Elastacloud.AzureManagement.Fluent.Commands.VirtualNetworks
{
    /// <summary>
    ///   Registers a virtual machine image for either Linux or Windowss     
    ///  </summary>
    public class GetVirtualNetworkAndSubnetsFromCloudServiceCommand : ServiceCommand
    {
        // https://management.core.windows.net/<subscription-id>/services/networking/<virtual-network-name>
        /// <summary>
        ///   Lists all images that are registered in your subscriptions   
        ///  </summary>
        internal GetVirtualNetworkAndSubnetsFromCloudServiceCommand(string cloudService)
        {
            AdditionalHeaders["x-ms-version"] = "2012-03-01";
            OperationId = "hostedservices";
            ServiceType = "services";
            HttpCommand = cloudService + "/deploymentslots/Production";
            HttpVerb = HttpVerbGet;
            NetworkDetails = new CloudServiceNetworking();
        }

        public CloudServiceNetworking NetworkDetails { get; private set; }

        /// <summary>
        /// Initially used via a response callback for commands which expect a async response 
        /// </summary>
        /// <param name="webResponse">the HttpWebResponse that will be sent back to the user from the request</param>
        protected override void ResponseCallback(HttpWebResponse webResponse)
        {
            XDocument document;

            using (var reader = new StreamReader(webResponse.GetResponseStream()))
            {
                document = XDocument.Parse(reader.ReadToEnd());
            }
            var root = document.Descendants(Namespaces.NsWindowsAzure + "SubnetName");
            NetworkDetails.Subnets = root.Select(subnet => subnet.Value).Distinct().ToList();
            var networkNode = document.Descendants(Namespaces.NsWindowsAzure + "VirtualNetworkName").FirstOrDefault();
            NetworkDetails.VirtualNetworkName = networkNode == null ? null : networkNode.Value;
            
            SitAndWait.Set();
        }

        /// <summary>
        /// returns the name of the command
        /// </summary>
        public override string ToString()
        {
            return "GetVirtualNetworkAndSubnetsFromCloudServiceCommand";
        }
    }
}
