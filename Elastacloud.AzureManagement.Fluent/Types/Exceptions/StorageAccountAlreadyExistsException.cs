using System;

namespace Elastacloud.AzureManagement.Fluent.Types.Exceptions
{
    /// <summary>
    /// Exception thrown when a storage account doesnot exist
    /// </summary>
    public class StorageAccountDoesNotExistException : ApplicationException
    {
        /// <summary>
        /// Uses the storage account name to construct a message for the user 
        /// </summary>
        public StorageAccountDoesNotExistException(string accountName)
            : base(String.Format("Account with name {0} does not exist", accountName))
        {
        }
    }
}