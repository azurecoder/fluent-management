/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

namespace Elastacloud.AzureManagement.Fluent.Storage
{
    /// <summary>
    /// The interface used for crud storage
    /// </summary>
    public interface IStorageAccount
    {
        /// <summary>
        /// Used to create a new storage account
        /// </summary>
        ICertificateActivity CreateNew(string name);

        /// <summary>
        /// Used to delete an existing storage account
        /// </summary>
        ICertificateActivity Delete(string name);

        /// <summary>
        /// A passthru method to get information on storage accounts
        /// </summary>
        ICertificateActivity ForStorageInformationQuery();
    }
}