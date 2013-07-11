/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System;
using System.Text;
using System.Xml.Linq;
using Elastacloud.AzureManagement.Fluent.Commands.Parsers;
using Elastacloud.AzureManagement.Fluent.Helpers;

namespace Elastacloud.AzureManagement.Fluent.Commands.Services
{
	/// <summary>
	/// Used to check the availablity of a hosted service
	/// </summary>
	internal class CheckCloudServiceNameAvailableCommand : ServiceCommand
	{
        // https://management.core.windows.net/<subscription-id>/services/hostedservices/operations/isavailable/<cloudservice-name>
        internal CheckCloudServiceNameAvailableCommand(string name)
		{
			Name = name;
			HttpVerb = HttpVerbGet;
			ServiceType = "services";
			OperationId = "hostedservices";
            HttpCommand = "operations/isavailable/" + name;
		}
        /// <summary>
        /// denotes whether the cloud service is available or not
        /// </summary>
	    public bool CloudServiceAvailable { get; set; }

        protected override void ResponseCallback(System.Net.HttpWebResponse webResponse)
        {
            CloudServiceAvailable = Parse(webResponse, BaseParser.CloudServiceAvailable,
                                              new CheckCloudServiceNameAvailableParser(null));
            SitAndWait.Set();
        }
	}
}