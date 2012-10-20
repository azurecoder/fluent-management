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