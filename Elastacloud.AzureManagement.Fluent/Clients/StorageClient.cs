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
using Elastacloud.AzureManagement.Fluent.Clients.Helpers;
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
        /// <summary>
        /// Used to construct a storage client with a subscription id and management certificate
        /// </summary>
        public StorageClient(string subscriptionId, X509Certificate2 certificate, string defaultLocation = LocationConstants.NorthEurope)
        {
            SubscriptionId = subscriptionId;
            ManagementCertificate = certificate;
            Location = defaultLocation;
        }
        /// <summary>
        /// Used to construct a client with an account name and account key
        /// </summary>
        public StorageClient(string accountName, string accountKey)
        {
            DefaultAccountName = accountName;
            DefaultAccountKey = accountKey;
        }
        /// <summary>
        /// This can be used to avoid a subcription id and management certificate
        /// </summary>
        public string DefaultAccountKey { get; set; }
        /// <summary>
        /// This can be used to avoid a subscription id and management certificate
        /// </summary>
        public string DefaultAccountName { get; set; }

        protected X509Certificate2 ManagementCertificate { get; set; }
        protected string SubscriptionId { get; set; }
        /// <summary>
        /// The default location for the storage accounts
        /// </summary>
        protected string Location { get; set; }
        /// <summary>
        /// Creates a new storage account given a name and location
        /// </summary>
        public void CreateNewStorageAccount(string name, string location = LocationConstants.NorthEurope
            , StorageManagementOptions options = null)
        {
            if (options == null)
                options = StorageManagementOptions.GetDefaultOptions;
            
            // issue the create storage account command 
            var create = new CreateStorageAccountCommand(name, "Created with Fluent Management", options, location)
                {
                    SubscriptionId = SubscriptionId,
                    Certificate = ManagementCertificate,
                    Location = Location
                };
            create.Execute();
            var status = StorageStatus.Creating;
            while (status != StorageStatus.Created)
            {
                var command = new GetStorageAccountStatusCommand(name)
                {
                    SubscriptionId = SubscriptionId,
                    Certificate = ManagementCertificate,
                    Location = Location
                };
                command.Execute();
                status = command.Status;
            }
        }

        /// <summary>
        /// Create the storage account if an account by the same name doesn't exist
        /// </summary>
		public void CreateStorageAccountIfNotExists(string name,
            string location = LocationConstants.NorthEurope, StorageManagementOptions options = null)
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
                Certificate = ManagementCertificate,
                Location = Location
            };
            delete.Execute();
        }

        public string[] GetStorageAccountKeys(string name)
        {
            var keys = new GetStorageAccountKeysCommand(name)
            {
                SubscriptionId = SubscriptionId,
                Certificate = ManagementCertificate,
                Location = Location
            };
            keys.Execute();
            return new string[2] {keys.PrimaryStorageKey, keys.SecondaryStorageKey};
        }

        public List<StorageAccount> GetStorageAccountList()
        {
            var getStorageAccountList = new ListStorageAccountsCommand
            {
                SubscriptionId = SubscriptionId,
                Certificate = ManagementCertificate,
                Location = Location
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
                Certificate = ManagementCertificate,
                Location = Location
            };
            storageAccounts.Execute();
            return storageAccounts.Status;
        }

        /// <summary>
        /// Gets a shared access signature given a full blob uri
        /// </summary>
        public string GetSaSFromBlobUri(string blobUri)
        {
            int accountPos = blobUri.IndexOf("http://", StringComparison.Ordinal) == 0 ? "http://".Length : "https://".Length;
            int firstPeriod = blobUri.IndexOf(".", StringComparison.Ordinal);
            string accountName = DefaultAccountName ?? blobUri.Substring(accountPos, firstPeriod - accountPos);
            string accountKey = DefaultAccountKey ?? GetStorageAccountKeys(accountName)[0];
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