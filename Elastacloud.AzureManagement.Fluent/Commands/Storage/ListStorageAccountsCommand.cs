/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System.Collections.Generic;
using System.Net;
using Elastacloud.AzureManagement.Fluent.Commands.Parsers;
using Elastacloud.AzureManagement.Fluent.Commands.Services;
using Elastacloud.AzureManagement.Fluent.Types;

namespace Elastacloud.AzureManagement.Fluent.Commands.Storage
{
    /// <summary>
    /// Used to create a hosted service within a given subscription
    /// </summary>
    // https://management.core.windows.net/<subscription-id>/services/storageservices/
    internal class ListStorageAccountsCommand : ServiceCommand
    {
        // https://management.core.windows.net/<subscription-id>/services/storageservices
        /// <summary>
        /// Constructs a command to list all of the storage accounts within a particular subscription
        /// </summary>
        internal ListStorageAccountsCommand()
        {
            HttpVerb = HttpVerbGet;
            ServiceType = "services";
            OperationId = "storageservices";
        }

        /// <summary>
        /// The list of storage accounts that are returned
        /// </summary>
        internal List<StorageAccount> StorageAccounts { get; set; }

        /// <summary>
        /// The response callback used parse the list of storage accounts
        /// </summary>
        /// <param name="webResponse">The Http web response</param>
        protected override void ResponseCallback(HttpWebResponse webResponse)
        {
            StorageAccounts = Parse(webResponse, BaseParser.ListStorageAccountsParser);
            SitAndWait.Set();
        }
    }
}