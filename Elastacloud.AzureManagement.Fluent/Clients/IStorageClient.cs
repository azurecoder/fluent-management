/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System.Collections.Generic;
using Elastacloud.AzureManagement.Fluent.Types;

namespace Elastacloud.AzureManagement.Fluent.Clients
{
    /// <summary>
    /// Defines an interface which can perform operations on blobs
    /// </summary>
    public interface IStorageClient
    {
        /// <summary>
        /// Creates a new storage account given a name and location
        /// </summary>
        void CreateNewStorageAccount(string name, string location = "North Europe");
        /// <summary>
        /// Create the storage account if an account by the same name doesn't exist
        /// </summary>
        void CreateStorageAccountIfNotExists(string name, string location = "North Europe");
        /// <summary>
        /// Deletes a storage account if it exists
        /// </summary>
        void DeleteStorageAccount(string name);
        /// <summary>
        /// Gets the primary and secondary keys of the storage account
        /// </summary>
        string[] GetStorageAccountKeys(string name);
        /// <summary>
        /// Gets a list of storage accounts 
        /// </summary>
        List<StorageAccount> GetStorageAccountList();
        /// <summary>
        /// Gets the status of a storage account
        /// </summary>
        StorageStatus GetStorageStatus(string name);
    }
}