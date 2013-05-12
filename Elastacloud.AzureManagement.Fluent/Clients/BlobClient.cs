/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System;
using System.Security.Cryptography.X509Certificates;
using Elastacloud.AzureManagement.Fluent.Clients.Interfaces;
using Elastacloud.AzureManagement.Fluent.Commands.Blobs;
using Elastacloud.AzureManagement.Fluent.Commands.Storage;
using Elastacloud.AzureManagement.Fluent.Types;

namespace Elastacloud.AzureManagement.Fluent.Clients
{
    public class BlobClient : IBlobClient
    {
        /// <summary>
        /// Used to construct the BlobClient
        /// </summary>
        public BlobClient(string subscriptionId, string containerName, string accountName, string accountKey)
        {
            SubscriptionId = subscriptionId;
            AccountName = accountName;
            AccountKey = accountKey;
            ContainerName = containerName;
        }

        /// <summary>
        /// This is the BlobClient and is used to create the account without the key and get the key automatically
        /// </summary>
        public BlobClient(string subscriptionId, string containerName, string accountName, X509Certificate2 certificate)
            : this(subscriptionId, containerName, accountName, string.Empty)
        {
            ManagementCertificate = certificate;
        }

        protected X509Certificate2 ManagementCertificate { get; set; }

        #region Implementation of IBlobClient

        /// <summary>
        /// Creates an uploads a blob given a file path
        /// </summary>
        /// <param name="blobName">the name of the blob to create</param>
        /// <param name="filenamePath">The name of the file to read an copy to blob storage</param>
        public string CreateAndUploadBlob(string blobName, string filenamePath)
        {
            LoadKeyIfNotExists();
            var blobCommand = new CreateAndUploadBlobCommand(ContainerName, blobName, filenamePath)
            {
                AccountKey = AccountKey,
                AccountName = AccountName
            };
            blobCommand.Execute();
            return blobCommand.DeploymentPath;
        }

        /// <summary>
        /// Deletes the current storage account
        /// </summary>
        public void DeleteStorageAccount()
        {
            var deleteStorage = new DeleteStorageAccountCommand(AccountName)
                                    {
                                        SubscriptionId = SubscriptionId,
                                        Certificate = ManagementCertificate
                                    };
            deleteStorage.Execute();
        }

        /// <summary>
        /// Creates a blob container given a valid container name
        /// </summary>
        public void CreatBlobContainer()
        {
            LoadKeyIfNotExists();
            var blobContainer = new CreateBlobContainerCommand(ContainerName)
            {
                AccountKey = AccountKey,
                AccountName = AccountName
            };
            blobContainer.Execute();
        }

        /// <summary>
        /// Deletes a blob 
        /// </summary>
        /// <param name="blobName">The name of a valid blob</param>
        public void DeleteBlob(string blobName)
        {
            LoadKeyIfNotExists();
            var deleteblob = new DeleteBlobCommand(ContainerName, blobName)
            {
                AccountName = AccountName,
                AccountKey = AccountKey
            };
            deleteblob.Execute();
        }

        /// <summary>
        /// Gets the blob account key
        /// </summary>
        public string GetAccountKey()
        {
            var getStorageAccountKeysCommand = new GetStorageAccountKeysCommand(AccountName)
            {
                SubscriptionId = SubscriptionId,
                Certificate = ManagementCertificate
            };
            getStorageAccountKeysCommand.Execute();
            return (AccountKey = getStorageAccountKeysCommand.PrimaryStorageKey);
        }

        /// <summary>
        /// Deletes a blob container
        /// </summary>
        public void DeleteContainer()
        {
            var blobContainer = new DeleteBlobContainerCommand(ContainerName)
            {
                AccountKey = AccountKey,
                AccountName = AccountName
            };
            blobContainer.Execute();
        }

        /// <summary>
        /// Checks to see whether a stroage account has completed a name resolution with an automatic checking timout of 5 minutes
        /// </summary>
        /// <returns>A boolean indicating whether the account has been created and resolved</returns>
        public bool CheckStorageAccountHasResolved(int timeoutInSeconds = 300)
        {
            // uses a blob command but can be any blob command
            var command = new DeleteBlobCommand(ContainerName, "")
            {
                AccountName = AccountName
            };
            return command.CheckStorageAccountExists(timeoutInSeconds);
        }

        /// <summary>
        /// The name of the container to which the client is bound
        /// </summary>
        public string ContainerName { get; private set; }

        /// <summary>
        /// The name of the storage account
        /// </summary>
        public string AccountName { get; private set; }

        /// <summary>
        /// The primary or secondary key used by the account
        /// </summary>
        public string AccountKey { get; private set; }

        /// <summary>
        /// The subscription id that the client is bound to 
        /// </summary>
        public string SubscriptionId { get; private set; }

        #endregion

        /// <summary>
        /// loads the account key if it is not present
        /// </summary>
        private void LoadKeyIfNotExists()
        {
            if (String.IsNullOrEmpty(AccountKey))
            {
                GetAccountKey();
            }
        }
    }
}
