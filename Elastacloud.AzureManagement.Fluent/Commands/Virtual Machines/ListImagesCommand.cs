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

namespace Elastacloud.AzureManagement.Fluent.Commands.VirtualMachines
{
    /// <summary>
    ///   Registers a virtual machine image for either Linux or Windowss     
    ///  </summary>
    public class ListImagesCommand : ServiceCommand
    {
        // https://management.core.windows.net/<subscription-id>/services/images        
        /// <summary>
        ///   Lists all images that are registered in your subscriptions   
        ///  </summary>
        internal ListImagesCommand()
        {
            AdditionalHeaders["x-ms-version"] = "2012-08-01";
            OperationId = "images";
            ServiceType = "services";
            HttpVerb = HttpVerbGet;
        }

        /// <summary>
        /// The full virtual machine properties of the windows instance the needs to be deployed
        /// </summary>
        public List<ImageProperties> Properties { get; set; }

        /// <summary>
        /// Initially used via a response callback for commands which expect a async response 
        /// </summary>
        /// <param name="webResponse">the HttpWebResponse that will be sent back to the user from the request</param>
        protected override void ResponseCallback(HttpWebResponse webResponse)
        {
            // get the cloud service deployments
            Properties = Parse(webResponse, "Images", new ListImagesParser(null));
            SitAndWait.Set();
        }

        /// <summary>
        /// returns the name of the command
        /// </summary>
        public override string ToString()
        {
            return "ListImagesCommand";
        }
    }
}
