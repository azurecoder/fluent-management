﻿/************************************************************************************************************
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
using Elastacloud.AzureManagement.Fluent.Clients.Interfaces;
using Elastacloud.AzureManagement.Fluent.Commands.Storage;
using Elastacloud.AzureManagement.Fluent.Helpers;
using Elastacloud.AzureManagement.Fluent.Types;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;

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

        public void CreateNewStorageAccount(string name, string location = LocationConstants.NorthEurope)
        {
            // issue the create storage account command 
            var create = new CreateStorageAccountCommand(name, "Created with Fluent Management", location)
                {
                    SubscriptionId = SubscriptionId,
                    Certificate = ManagementCertificate
                };
            create.Execute();
        }

        /// <summary>
        /// Create the storage account if an account by the same name doesn't exist
        /// </summary>
		public void CreateStorageAccountIfNotExists(string name, string location = LocationConstants.NorthEurope)
        {
            if (GetStorageAccountList().All(a => a.Name != name))
            {
                CreateNewStorageAccount(name, location);
            }
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

        public List<StorageAccount> GetStorageAccountListWithKeys()
        {
            var newAccounts = new List<StorageAccount>();
            var accounts = GetStorageAccountList();
            foreach (var account in accounts)
            {
                var keys = GetStorageAccountKeys(account.Name);
                account.PrimaryAccessKey = keys[0];
                account.SecondaryAccessKey = keys[1];
                newAccounts.Add(account);
            }
            return newAccounts;
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

        /// <summary>
        /// Gets a shared access signature given a full blob uri
        /// </summary>
        public string GetSaSFromBlobUri(string blobUri)
        {
            int accountPos = blobUri.IndexOf("http://", StringComparison.Ordinal) > 0 ? "http://".Length + 1 : "https://".Length + 1;
            int firstPeriod = blobUri.IndexOf(".", StringComparison.Ordinal);
            string accountName = blobUri.Substring(accountPos, firstPeriod - accountPos);
            string accountKey = GetStorageAccountKeys(accountName)[0];
            var account = new CloudStorageAccount(new StorageCredentials(accountName, accountKey), true);
            var blobClient = account.CreateCloudBlobClient();
            var blobReference = blobClient.GetBlobReferenceFromServer(new Uri(blobUri));
            string sas = blobReference.GetSharedAccessSignature(new SharedAccessBlobPolicy()
            {
                Permissions = SharedAccessBlobPermissions.Read,
                SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(90),
                SharedAccessStartTime = DateTime.UtcNow.AddMinutes(-5)
            });
            return blobUri + sas;
        }
    }
}