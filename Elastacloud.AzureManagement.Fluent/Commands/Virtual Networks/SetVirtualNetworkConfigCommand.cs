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
using System.IO;
using System.Net;
using System.Xml.Linq;
using Elastacloud.AzureManagement.Fluent.Commands.Parsers;
using Elastacloud.AzureManagement.Fluent.Commands.Services;
using Elastacloud.AzureManagement.Fluent.Helpers;
using Elastacloud.AzureManagement.Fluent.Types.VirtualMachines;
using Elastacloud.AzureManagement.Fluent.Types.VirtualNetworks;

namespace Elastacloud.AzureManagement.Fluent.Commands.VirtualNetworks
{
    /// <summary>
    ///   Registers a virtual machine image for either Linux or Windowss     
    ///  </summary>
    public class SetVirtualNetworkConfigCommand : ServiceCommand
    {
        // https://management.core.windows.net/<subscription-id>/services/networking/media     
        /// <summary>
        ///   Gets the net config for the specific virtual network
        ///  </summary>
        internal SetVirtualNetworkConfigCommand(string xmlConfigDocument)
        {
            AdditionalHeaders["x-ms-version"] = "2014-02-01";
            ContentType = "text/plain";
            OperationId = "networking/media";
            ServiceType = "services";
            HttpVerb = HttpVerbPut;
            XmlConfigDocument = xmlConfigDocument;
        }

        public string XmlConfigDocument { get; set; }

        protected override string CreatePayload()
        {
            return XmlConfigDocument;
        }

        /// <summary>
        /// returns the name of the command
        /// </summary>
        public override string ToString()
        {
            return "SetAllVirtualNetworkConfigCommand";
        }
    }
}
