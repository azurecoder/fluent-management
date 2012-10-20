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
using Elastacloud.AzureManagement.Fluent.Commands.Storage;
using Elastacloud.AzureManagement.Fluent.Types;
using Elastacloud.AzureManagement.Fluent.Types.Exceptions;

namespace Elastacloud.AzureManagement.Fluent.Storage.Classes
{
    /// <summary>
    /// Encapsulates the storage activity - all of the storage manager is just a passthru to this class
    /// </summary>
    internal class StorageActivity : IStorageActivity, IServiceTransaction
    {
        /// <summary>
        /// The manager variable - used to back reference the manager
        /// </summary>
        internal StorageManager Manager;

        /// <summary>
        /// Used to a hold a value for whether the transaction has started and is successful
        /// </summary>
        private bool _started;

        /// <summary>
        /// The storage account being returned by the command
        /// </summary>
        private StorageAccount _storageAccount;

        /// <summary>
        /// Used to a hold a value for whether the transaction has started and is successful
        /// </summary>
        private bool _success;

        /// <summary>
        /// Constructor used to set the manager reference 
        /// </summary>
        internal StorageActivity(StorageManager manager)
        {
            Manager = manager;
        }

        #region IStorageActivity Members

        /// <summary>
        /// A description for the new storage account
        /// </summary>
        IStorageActivity IStorageActivity.WithDescription(string description)
        {
            Manager.StorageAccountDescription = description;
            return this;
        }

        /// <summary>
        /// the location for the new storage account
        /// </summary>
        IStorageActivity IStorageActivity.WithLocation(string location)
        {
            Manager.StorageAccountLocation = location;
            return this;
        }

        /// <summary>
        /// Gets a list of storage accounts from the storage catalog
        /// </summary>
        List<StorageAccount> IStorageActivity.GetStorageAccountList(bool includeKeys)
        {
            var getStorageAccountList = new ListStorageAccountsCommand
                                            {
                                                SubscriptionId = Manager.SubscriptionId,
                                                Certificate = Manager.ManagementCertificate
                                            };
            getStorageAccountList.Execute();
            if(!includeKeys)
                return getStorageAccountList.StorageAccounts;
            // if the get keys flag is supplied then ensure that this returns the keys with the structure
            foreach (var storageAccount in getStorageAccountList.StorageAccounts)
            {
                var keys = new GetStorageAccountKeysCommand(storageAccount.Name)
                    {
                        SubscriptionId = Manager.SubscriptionId,
                        Certificate = Manager.ManagementCertificate
                    };
                keys.Execute();
                storageAccount.PrimaryAccessKey = keys.PrimaryStorageKey;
                storageAccount.SecondaryAccessKey = keys.SecondaryStorageKey;
            }
            return getStorageAccountList.StorageAccounts;
        }

        /// <summary>
        /// The method used to execute and determine what operation to do with the storage account
        /// </summary>
        IServiceTransaction IStorageActivity.Go()
        {
            return this;
        }

        #endregion

        /// <summary>
        /// Used to delete a storage account
        /// </summary>
        private bool DeleteStorageAccount()
        {
            try
            {
                var delete = new DeleteStorageAccountCommand(Manager.StorageAccountName)
                                 {
                                     SubscriptionId = Manager.SubscriptionId,
                                     Certificate = Manager.ManagementCertificate
                                 };
                delete.Execute();
                Manager.WriteComplete(EventPoint.StorageAccountCreated,
                                      "Storage account " + Manager.StorageAccountName + " created");
            }
            catch (Exception)
            {
                Manager.WriteComplete(EventPoint.StorageAccountCreated,
                                      "Storage account " + Manager.StorageAccountName + " not created");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Used to create a storage account
        /// </summary>
        private bool CreateStorageAccount()
        {
            // Check to see whether the storage account details have been populated correctly
            if (Manager.StorageAccountDescription == null || Manager.StorageAccountLocation == null)
            {
                Manager.WriteComplete(EventPoint.ExceptionOccurrence, "Description or location not specified");
                return false;
            }

            try
            {
                // issue the create storage account command 
                var create = new CreateStorageAccountCommand(Manager.StorageAccountName,
                                                             Manager.StorageAccountDescription,
                                                             Manager.StorageAccountLocation)
                                 {
                                     SubscriptionId = Manager.SubscriptionId,
                                     Certificate = Manager.ManagementCertificate
                                 };
                create.Execute();
                Manager.WriteComplete(EventPoint.StorageAccountCreated,
                                      "Storage account " + Manager.StorageAccountName + " created");
                // get the storage account keys 
                var keys = new GetStorageAccountKeysCommand(Manager.StorageAccountName)
                               {
                                   SubscriptionId = Manager.SubscriptionId,
                                   Certificate = Manager.ManagementCertificate
                               };
                keys.Execute();
                Manager.WriteComplete(EventPoint.StorageKeyRequestSuccess, "Keys returned from storage request");
                // populate the primary and secondary keys 
                Manager.StorageAccountPrimaryKey = keys.PrimaryStorageKey;
                Manager.StorageAccountSecondaryKey = keys.SecondaryStorageKey;
            }
            catch (Exception exception)
            {
                // rollback the operation if it failed
                Manager.WriteComplete(EventPoint.ExceptionOccurrence, exception.GetType() + ": " + exception.Message);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Finds a storage account with a particular name
        /// </summary>
        private StorageAccount FindAccounByName(string name)
        {
            List<StorageAccount> storageAccounts = ((IStorageActivity) this).GetStorageAccountList();
            return storageAccounts.Find(a => a.Name == name);
        }

        #region Implementation of IServiceTransaction

        /// <summary>
        /// Used to commit the transaction data 
        /// </summary>
        /// <returns>A dynamic type which represents the return of the particular transaction</returns>
        dynamic IServiceTransaction.Commit()
        {
            // set the start flag
            _started = true;
            // ensure that the account has been given a name
            if (Manager.StorageAccountName == null)
                throw new ApplicationException(
                    "unable to continue - no storage account name present. Did you want to query instead?");

            // get the list of accounts and check to see whether the account already exists
            _storageAccount = FindAccounByName(Manager.StorageAccountName);
            // Check to see whether the operation is create or delete - we only want to support a transaction on the create
            // this API does not follow a crud methodology
            if (Manager.CreateNewStorageAccount)
            {
                if (_storageAccount != null)
                    throw new StorageAccountAlreadyExistsException(Manager.StorageAccountName);
                // create the account depending on the flag that has been set
                _success = CreateStorageAccount();
            }
            else
            {
                if (_storageAccount == null)
                    throw new StorageAccountDoesNotExistException(Manager.StorageAccountName);
                // delete the account depending on the flag that has been set
                _success = DeleteStorageAccount();
            }
            return _success;
        }

        /// <summary>
        /// Used to rollback the transaction in the event of failure 
        /// </summary>
        public void Rollback()
        {
            // do a null check on the account we don't want to fail this bit
            StorageAccount account = FindAccounByName(Manager.StorageAccountName);

            if (Manager.CreateNewStorageAccount)
            {
                // we need to be able to do the reverse operation here 
                if (account != null)
                    DeleteStorageAccount();
            }
            else
            {
                // we need to be able to do the reverse operation here to 
                if (account == null)
                    CreateStorageAccount();
            }
        }

        /// <summary>
        /// Used to denote whether the transaction has been started or not
        /// </summary>
        public bool Started
        {
            get { return _started; }
        }

        /// <summary>
        /// Used to denote whether the transaction has succeeded or not
        /// </summary>
        public bool Succeeded
        {
            get { return _success; }
        }

        #endregion
    }
}