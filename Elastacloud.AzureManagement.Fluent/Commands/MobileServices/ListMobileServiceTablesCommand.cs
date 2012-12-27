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
using Elastacloud.AzureManagement.Fluent.Commands.Parsers;
using Elastacloud.AzureManagement.Fluent.Types.Exceptions;
using Elastacloud.AzureManagement.Fluent.Types.MobileServices;

namespace Elastacloud.AzureManagement.Fluent.Commands.MobileServices
{
    /// <summary>
    /// Used to create a mobile services application
    /// </summary>
    internal class ListMobileServiceTablesCommand : MobileServiceCommand
    {
        /// <summary>
        /// Constructs a command to create a mobile service
        /// </summary>
        internal ListMobileServiceTablesCommand(string serviceName)
        {
            OperationId = "mobileservices";
            ServiceType = "services";
            Name = serviceName;
            HttpVerb = HttpVerbGet;
            HttpCommand = string.Format("mobileservices/{0}/tables", serviceName.ToLower());
        }

        /// <summary>
        /// The tables brought back from mobile services
        /// </summary>
        public List<MobileServiceTable> Tables { get; set; }

        /// <summary>
        /// Used to return the mobile service application details 
        /// </summary>
        protected override void ResponseCallback(System.Net.HttpWebResponse webResponse)
        {
            Tables = Parse(webResponse, BaseParser.ListMobileServiceTablesParser, new ListMobileServicesTableParser(null));
            
            SitAndWait.Set();
        }

        /// <summary>
        /// Returns the string name of the command
        /// </summary>
        public override string ToString()
        {
            return "ListMobileServiceTablesCommand";
        }
    }
}