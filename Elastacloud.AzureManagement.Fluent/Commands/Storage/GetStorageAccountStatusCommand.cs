/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System;
using System.Net;
using System.Xml.Linq;
using Elastacloud.AzureManagement.Fluent.Commands.Services;
using Elastacloud.AzureManagement.Fluent.Types;

namespace Elastacloud.AzureManagement.Fluent.Commands.Storage
{
    /// <summary>
    /// Used to create a hosted service within a given subscription
    /// </summary>
    // https://management.core.windows.net/<subscription-id>/services/storageservices/<service-name>
    internal class GetStorageAccountStatusCommand : ServiceCommand
    {
        internal GetStorageAccountStatusCommand(string name)
        {
            Name = name;
            HttpVerb = HttpVerbGet;
            ServiceType = "services";
            OperationId = "storageservices";
            HttpCommand = name;
        }

        public StorageStatus Status { get; set; }

        protected override void ResponseCallback(HttpWebResponse webResponse)
        {
            XNamespace ns = "http://schemas.microsoft.com/windowsazure";
            XDocument document = XDocument.Load(webResponse.GetResponseStream());
            XElement elementStorageStatus = document.Element(ns + "StorageService").Element(ns + "StorageServiceProperties")
                .Element(ns + "Status");
            Status = (StorageStatus)Enum.Parse(typeof(StorageStatus), elementStorageStatus.Value);
        
            SitAndWait.Set();
        }
    }
}