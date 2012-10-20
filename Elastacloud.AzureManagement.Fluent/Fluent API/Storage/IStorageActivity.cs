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

namespace Elastacloud.AzureManagement.Fluent.Storage
{
    /// <summary>
    /// Used to desribe the storage account that is being created or deleted
    /// </summary>
    public interface IStorageActivity
    {
        /// <summary>
        /// Used to add the account description to the storage account
        /// </summary>
        IStorageActivity WithDescription(string description);

        /// <summary>
        /// Adds the location of the account for the storage creation 
        /// </summary>
        IStorageActivity WithLocation(string location);

        /// <summary>
        /// Gets a list of all of the storage accounts within the subscription
        /// </summary>
        List<StorageAccount> GetStorageAccountList(bool includeKeys = false);

        /// <summary>
        /// Used to execute the activity
        /// </summary>
        /// <returns>An IServiceTransaction Interface used to turn this into a transaction</returns>
        IServiceTransaction Go();
    }
}