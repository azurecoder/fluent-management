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
using System.Text;
using Elastacloud.AzureManagement.Fluent.Commands.Storage;
using Elastacloud.AzureManagement.Fluent.Types;

namespace Elastacloud.AzureManagement.Fluent.Clients
{
    /// <summary>
    /// Defines a class which can perform operations on blobs
    /// </summary>
    public class StorageClient : IStorageClient
    {
        public StorageClient(string subscriptionId, X509Certificate2 certificate)
        {
            SubscriptionId = subscriptionId;
            ManagementCertificate = certificate;
        }

        protected X509Certificate2 ManagementCertificate { get; set; }
        protected string SubscriptionId { get; set; }

        public void CreateNewStorageAccount(string name, string location = "North Europe")
        {
            // issue the create storage account command 
            var create = new CreateStorageAccountCommand(name, "Created with Fluent Management", location)
            {
                SubscriptionId = SubscriptionId,
                Certificate = ManagementCertificate
            };
            create.Execute();
        }

        public void DeleteStorageAccount(string name)
        {
            var delete = new DeleteStorageAccountCommand(name)
            {
                SubscriptionId = SubscriptionId,
                Certificate = ManagementCertificate
            };
            delete.Execute();
        }

        public string[] GetStorageAccountKeys(string name)
        {
            var keys = new GetStorageAccountKeysCommand(name)
            {
                SubscriptionId = SubscriptionId,
                Certificate = ManagementCertificate
            };
            keys.Execute();
            return new string[2] {keys.PrimaryStorageKey, keys.SecondaryStorageKey};
        }

        public List<StorageAccount> GetStorageAccountList()
        {
            var getStorageAccountList = new ListStorageAccountsCommand
            {
                SubscriptionId = SubscriptionId,
                Certificate = ManagementCertificate
            };
            getStorageAccountList.Execute();
            return getStorageAccountList.StorageAccounts;
        }

        public StorageStatus GetStorageStatus(string name)
        {
            var storageAccounts = new GetStorageAccountStatusCommand(name)
            {
                SubscriptionId = SubscriptionId,
                Certificate = ManagementCertificate
            };
            storageAccounts.Execute();
            return storageAccounts.Status;
        }
    }
}