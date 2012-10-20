/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System.Net;
using System.Xml.Linq;
using Elastacloud.AzureManagement.Fluent.Commands.Services;

namespace Elastacloud.AzureManagement.Fluent.Commands.Storage
{
    /// <summary>
    /// Used to create a hosted service within a given subscription
    /// </summary>
    // https://management.core.windows.net/<subscription-id>/services/storageservices/<service-name>/keys
    internal class GetStorageAccountKeysCommand : ServiceCommand
    {
        internal GetStorageAccountKeysCommand(string name)
        {
            Name = name;
            HttpVerb = HttpVerbGet;
            ServiceType = "services";
            OperationId = "storageservices";
            HttpCommand = name + "/keys";
        }

        public string PrimaryStorageKey { get; set; }
        public string SecondaryStorageKey { get; set; }

        protected override void ResponseCallback(HttpWebResponse webResponse)
        {
            XNamespace ns = "http://schemas.microsoft.com/windowsazure";
            XDocument document = XDocument.Load(webResponse.GetResponseStream());
            XElement elementStorageServiceKey = document.Element(ns + "StorageService")
                .Element(ns + "StorageServiceKeys");
            PrimaryStorageKey = (string) elementStorageServiceKey.Element(ns + "Primary");
            SecondaryStorageKey = (string) elementStorageServiceKey.Element(ns + "Secondary");
            SitAndWait.Set();
        }
    }
}