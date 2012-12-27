/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System;

namespace Elastacloud.AzureManagement.Fluent.Types.Exceptions
{
    /// <summary>
    /// Exception thrown when a storage account exists 
    /// </summary>
    public class StorageAccountAlreadyExistsException : ApplicationException
    {
        /// <summary>
        /// Uses the storage account name to construct a message for the user 
        /// </summary>
        public StorageAccountAlreadyExistsException(string accountName)
            : base(String.Format("Account with name {0} already exists", accountName))
        {
        }
    }
}