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
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Elastacloud.AzureManagement.Fluent.Clients.Interfaces;
using Elastacloud.AzureManagement.Fluent.Commands.Blobs;
using Elastacloud.AzureManagement.Fluent.Commands.Storage;
using Elastacloud.AzureManagement.Fluent.Types;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;

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

        /// <summary>
        /// Used to create a blob client with an account name and key
        /// </summary>
        public BlobClient(string accountName, string accountKey)
        {
            AccountName = accountName;
            AccountKey = accountKey;
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
        public bool CreatBlobContainer()
        {
            LoadKeyIfNotExists();
            var blobContainer = new CreateBlobContainerCommand(ContainerName)
            {
                AccountKey = AccountKey,
                AccountName = AccountName
            };
            try
            {
                blobContainer.Execute();
            }
            catch
            {
                return false;
            }
            return true;
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
        /// Used to enable storage analytics for a particular blob type
        /// </summary>
        public void EnableStorageAnalytics(AnalyticsMetricsType metricsType = AnalyticsMetricsType.Logging)
        {
            var command = new EnableStorageAnalyticsCommand(StorageServiceType.Blob, metricsType)
            {
                AccountName = AccountName,
                AccountKey = AccountKey
            };
            command.Execute();
        }

        /// <summary>
        /// Used to enable storage analytics for a particular blob type
        /// </summary>
        public bool IsStorageAnalyticsEnabled(AnalyticsMetricsType metricsType = AnalyticsMetricsType.Logging)
        {
            var command = new GetStorageAnalyticsEnabledCommand(StorageServiceType.Blob, metricsType)
            {
                AccountName = AccountName,
                AccountKey = AccountKey
            };
            command.Execute();
            return command.StorageAnalyticsEnabled;
        }

        /// <summary>
        /// Given the shared access signature of a container generates a set blob SaS's
        /// </summary>
        public void CopyBlobsFromContainerSas(string containerSas, string destinationContainer)
        {
            var container = new CloudBlobContainer(new Uri(containerSas));
            var blobs = container.ListBlobs("blob/2014/08/07", true);

            var destinationAccountStorageCredentials = new StorageCredentials(AccountName, AccountKey);
            var destinationAccount = new CloudStorageAccount(destinationAccountStorageCredentials, true);
            var destinationBlobClient = destinationAccount.CreateCloudBlobClient();
            var destinationContainerInner = destinationBlobClient.GetContainerReference(destinationContainer);
            foreach (var blob in blobs)
            {
                var blockBlob = blob as CloudBlockBlob;
                using(var reader = new StreamReader(blockBlob.OpenRead()))
                {
                    var destinationBlob = destinationContainerInner.GetBlockBlobReference(blockBlob.Name);
                    string contents = reader.ReadToEnd();

                    using (var writer = new StreamWriter(destinationBlob.OpenWrite()))
                    {
                        writer.Write(contents);
                    }
                }
            }
          
          
        }

        /// <summary>
        /// Copies the log directory to the 
        /// </summary>
        public async Task<CopyableBlob[]> CopyDirectoryTo(string accountName, string accountKey,
            string sourceContainerName, string directoryName, string destinationContanerName, string copyDirectoryPrefix = "")
        {
            var copyIds = new List<CopyableBlob>();
            // get a list of the source account blobs
            var sourceAccountStorageCredentials = new StorageCredentials(AccountName, AccountKey);
            var account = new CloudStorageAccount(sourceAccountStorageCredentials, true);
            var client = account.CreateCloudBlobClient();
            var sourceContainer = client.GetContainerReference(sourceContainerName);
            var blobs = sourceContainer.ListBlobs(directoryName, true);
            // get a list of the destination account blobs 
            var destinationAccountCredentials = new StorageCredentials(accountName, accountKey);
            var destinationAccount = new CloudStorageAccount(destinationAccountCredentials, true);
            var destinationClient = destinationAccount.CreateCloudBlobClient();
            var destinationContainer = destinationClient.GetContainerReference(destinationContanerName);
            destinationContainer.CreateIfNotExists();
            foreach (var blob in blobs)
            {
                var blockBlob = blob as CloudBlockBlob;
                var destinationBlockBlob =
                    destinationContainer.GetBlockBlobReference(String.Format("{0}/{1}", copyDirectoryPrefix, 
                        String.Join("", blob.Uri.Segments.Skip(3).Take(5).ToArray())));
                var sas = blockBlob.GetSharedAccessSignature(new SharedAccessBlobPolicy()
                {
                    Permissions = SharedAccessBlobPermissions.Read,
                    SharedAccessStartTime = DateTime.Now.AddMinutes(-15),
                    SharedAccessExpiryTime = DateTime.Now.AddHours(1)
                });
                blockBlob = new CloudBlockBlob(new Uri(blockBlob.Uri.AbsoluteUri + sas));
  
                copyIds.Add(new CopyableBlob()
                {
                    // the use of a SAS fubars this 
                    CopyId = await destinationBlockBlob.StartCopyFromBlobAsync(blockBlob),
                    BlobUri = destinationBlockBlob.Uri,
                    ContainerName = destinationContanerName,
                    Size = (double) blockBlob.Properties.Length/1024,
                    PercentageCopied = 0
                });
            }
            CopyingBlobs = copyIds.ToArray();

            int completed = 0;
            while (true)
            {
                foreach (var blob in copyIds)
                {
                    var blockBlob = new CloudBlockBlob(blob.BlobUri, destinationAccountCredentials);
                    await blockBlob.FetchAttributesAsync();
                    await Task.Delay(500);
                    if (blockBlob.CopyState.BytesCopied.HasValue)
                    {
                        blob.PercentageCopied =
                            Convert.ToInt32(
                                Math.Round(
                                    (double) blockBlob.CopyState.BytesCopied.Value/blockBlob.CopyState.TotalBytes.Value,
                                    2)*100);
                    }
                    if (blockBlob.CopyState.Status == CopyStatus.Success ||
                        blockBlob.CopyState.Status == CopyStatus.Failed)
                    {
                        completed++;
                    }
                    if (completed == copyIds.Count)
                        return CopyingBlobs;
                }
            }
        }

        /// <summary>
        /// Copies the storage analytics logs to a new storage account
        /// </summary>
        public Task<CopyableBlob[]> CopyStorageAnalyticsLogsTo(string accountName, string accountKey, string destinationContainer, string sourceDirectory, string prefix)
        {
            return CopyDirectoryTo(accountName, accountKey, "$logs", sourceDirectory, destinationContainer, prefix);
        }

        /// <summary>
        /// Gets a list of the blobs that are copying
        /// </summary>
        public CopyableBlob[] CopyingBlobs { private set; get; }

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
