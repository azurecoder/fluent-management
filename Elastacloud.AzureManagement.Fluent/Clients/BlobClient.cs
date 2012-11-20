using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Elastacloud.AzureManagement.Fluent.Commands.Blobs;
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

        #region Implementation of IBlobClient

        /// <summary>
        /// Creates an uploads a blob given a file path
        /// </summary>
        /// <param name="blobName">the name of the blob to create</param>
        /// <param name="filenamePath">The name of the file to read an copy to blob storage</param>
        public string CreateAndUploadBlob(string blobName, string filenamePath)
        {
            var blobCommand = new CreateAndUploadBlobCommand(ContainerName, blobName, filenamePath)
            {
                AccountKey = AccountName,
                AccountName = AccountKey
            };
            blobCommand.Execute();
            return blobCommand.DeploymentPath;
        }

        /// <summary>
        /// Creates a blob container given a valid container name
        /// </summary>
        public void CreatBlobContainer()
        {
            var blobContainer = new CreateBlobContainerCommand(ContainerName)
            {
                AccountKey = AccountName,
                AccountName = AccountKey
            };
            blobContainer.Execute();
        }

        /// <summary>
        /// Deletes a blob 
        /// </summary>
        /// <param name="blobName">The name of a valid blob</param>
        public void DeleteBlob(string blobName)
        {
            var deleteblob = new DeleteBlobCommand(ContainerName, blobName)
            {
                AccountName = AccountName,
                AccountKey = AccountKey
            };
            deleteblob.Execute();
        }

        /// <summary>
        /// Deletes a blob container
        /// </summary>
        public void DeleteContainer()
        {
            var blobContainer = new DeleteBlobContainerCommand(ContainerName)
            {
                AccountKey = AccountName,
                AccountName = AccountKey
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
    }
}
