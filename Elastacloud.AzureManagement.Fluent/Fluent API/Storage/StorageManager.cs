/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Elastacloud.AzureManagement.Fluent.Helpers;
using Elastacloud.AzureManagement.Fluent.Helpers.PublishSettings;
using Elastacloud.AzureManagement.Fluent.Storage.Classes;
using Elastacloud.AzureManagement.Fluent.Types;

namespace Elastacloud.AzureManagement.Fluent.Storage
{
    /// <summary>
    /// Used to create or delete storage accounts 
    /// </summary>
    public class StorageManager : IAzureManager, ICertificateActivity, IStorageActivity, IStorageAccount
    {
        /// <summary>
        /// Used to tell whether a new storage account is being or not - by default false which means that the storage account with 
        /// {name} will be deleted
        /// </summary>
        public bool CreateNewStorageAccount;

        #region Implementation of IAzureManager

        public event EventReached AzureTaskComplete;
        public string SubscriptionId { get; set; }
        public X509Certificate2 ManagementCertificate { get; set; }

        #endregion

        /// <summary>
        /// Used to construct for a particular subscription
        /// </summary>
        internal StorageManager(string subscriptionId)
        {
            SubscriptionId = subscriptionId;
        }

        #region Implementation of ICertificateActivity

        /// <summary>
        /// Adds a .publishsettings file to get the certificate extract
        /// </summary>
        IStorageActivity ICertificateActivity.AddPublishSettings(string path)
        {
            var settings = new PublishSettingsExtractor(path);
            ManagementCertificate = settings.GetCertificateFromFile();
            return this;
        }

        /// <summary>
        /// Adds a certificate from a store using the thumbprint
        /// </summary>
        IStorageActivity ICertificateActivity.AddCertificateFromStore(string thumbprint)
        {
            ManagementCertificate = PublishSettingsExtractor.FromStore(thumbprint);
            return this;
        }

        /// <summary>
        /// Adds an X509 certificate from a .NET X509Certificate2 object
        /// </summary>
        IStorageActivity ICertificateActivity.AddCertificate(X509Certificate2 certificate)
        {
            ManagementCertificate = certificate;
            return this;
        }

        #endregion

        #region Implementation of IStorageActivity

        /// <summary>
        /// Used to add a description to the hosted service and the deployment
        /// </summary>
        IStorageActivity IStorageActivity.WithDescription(string description)
        {
            IStorageActivity activity = new StorageActivity(this);
            return activity.WithDescription(description);
        }

        /// <summary>
        /// Used to add a location to the hosted service and the deployment
        /// </summary>
        IStorageActivity IStorageActivity.WithLocation(string location)
        {
            IStorageActivity activity = new StorageActivity(this);
            return activity.WithLocation(location);
        }

        /// <summary>
        /// Gets a list of all the available storage accounts within the subscription
        /// </summary>
        List<StorageAccount> IStorageActivity.GetStorageAccountList(bool includeKeyDetails)
        {
            IStorageActivity activity = new StorageActivity(this);
            return activity.GetStorageAccountList(includeKeyDetails);
        }

        /// <summary>
        /// Used to execute the activity
        /// </summary>
        /// <returns>An IServiceTransaction Interface used to turn this into a transaction</returns>
        IServiceTransaction IStorageActivity.Go()
        {
            IStorageActivity activity = new StorageActivity(this);
            return activity.Go();
        }

        #endregion

        public void WriteComplete(EventPoint point, string message)
        {
            if (AzureTaskComplete != null)
                AzureTaskComplete(point, message);
        }

        #region Implementation of IStorageAccount

        /// <summary>
        /// The name of the storage account
        /// </summary>
        public string StorageAccountName { get; set; }

        /// <summary>
        /// The primary key created by the storage account create transaction
        /// </summary>
        public string StorageAccountPrimaryKey { get; set; }

        /// <summary>
        /// The secondary key created by the creation of the storage account
        /// </summary>
        public string StorageAccountSecondaryKey { get; set; }

        /// <summary>
        /// The storage account description 
        /// </summary>
        public string StorageAccountDescription { get; set; }

        /// <summary>
        /// The storage account location 
        /// </summary>
        public string StorageAccountLocation { get; set; }

        /// <summary>
        /// Creates a new instance of a storage account 
        /// </summary>
        /// <param name="name">The name of the account</param>
        /// <returns>an ICertificateActivity interface to promote the use of a management certificate</returns>
        public ICertificateActivity CreateNew(string name)
        {
            StorageAccountName = name.ToLower();
            CreateNewStorageAccount = true;
            return this;
        }

        /// <summary>
        /// Used to delete a storage account
        /// </summary>
        /// <param name="name">The name of the account</param>
        /// <returns>An ICertificateActivity interface used to return certificate activity</returns>
        public ICertificateActivity Delete(string name)
        {
            StorageAccountName = name.ToLower();
            CreateNewStorageAccount = false;
            return this;
        }

        /// <summary>
        /// A passthrough method to allow a query to be run against a storage account
        /// </summary>
        /// <returns>an ICertificateActivity interface to promote the use of a management certificate</returns>
        public ICertificateActivity ForStorageInformationQuery()
        {
            return this;
        }

        #endregion
    }
}