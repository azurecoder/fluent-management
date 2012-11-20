using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Elastacloud.AzureManagement.Fluent.Clients
{
    /// <summary>
    /// Defines an interface which can perform operations on blobs
    /// </summary>
    public interface IBlobClient
    {
        /// <summary>
        /// Creates an uploads a blob given a file path
        /// </summary>
        /// <param name="blobName">the name of the blob to create</param>
        /// <param name="filenamePath">The name of the file to read an copy to blob storage</param>
        string CreateAndUploadBlob(string blobName, string filenamePath);

        /// <summary>
        /// Creates a blob container given a valid container name
        /// </summary>
        void CreatBlobContainer();

        /// <summary>
        /// Deletes a blob 
        /// </summary>
        /// <param name="blobName">The name of a valid blob</param>
        void DeleteBlob(string blobName);

        /// <summary>
        /// Deletes a blob container
        /// </summary>
        void DeleteContainer();

        /// <summary>
        /// Checks to see whether a stroage account has completed a name resolution with an automatic checking timout of 5 minutes
        /// </summary>
        /// <returns>A boolean indicating whether the account has been created and resolved</returns>
        bool CheckStorageAccountHasResolved(int timeoutInSeconds = 300);

        /// <summary>
        /// The name of the container to which the client is bound
        /// </summary>
        string ContainerName { get; }

        /// <summary>
        /// The name of the storage account
        /// </summary>
        string AccountName { get; }

        /// <summary>
        /// The primary or secondary key used by the account
        /// </summary>
        string AccountKey { get; }

        /// <summary>
        /// The subscription id that the client is bound to 
        /// </summary>
        string SubscriptionId { get; }
    }
}