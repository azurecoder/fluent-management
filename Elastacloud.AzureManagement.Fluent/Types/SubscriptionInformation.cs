/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

namespace Elastacloud.AzureManagement.Fluent.Types
{
    /// <summary>
    /// A structure used to describe the subscription information returned by Windows Azure
    /// </summary>
    public class SubscriptionInformation
    {
        /// <summary>
        /// The Guid subscription id defining your Azure services
        /// </summary>
        public string SubscriptionId { get; set; }

        /// <summary>
        /// The associated name of the subscription - e.g. MSDN ... or subscription1 ..
        /// </summary>
        public string SubscriptionName { get; set; }

        /// <summary>
        /// Takes the values Active or Disabled
        /// </summary>
        public SubscriptionStatus SubscriptionStatus { get; set; }

        /// <summary>
        /// The live id of the account administator - reponsible for billing
        /// </summary>
        public string AccountAdminLiveId { get; set; }

        /// <summary>
        /// The live id of the service administator if different
        /// </summary>
        public string ServiceAdminLiveId { get; set; }

        /// <summary>
        /// The total number of cores available to the subscription - will vary between EA subscriptiona and Pay as you Create (normally 20)
        /// </summary>
        public int MaxCoreCount { get; set; }

        /// <summary>
        /// The total number of storage accounts available to the subscription (normally 5)
        /// </summary>
        public int MaxStorageAccounts { get; set; }

        /// <summary>
        /// The total number of hosted service available to the subscription (normally 6)
        /// </summary>
        public int MaxHostedServices { get; set; }

        /// <summary>
        /// The current number of cores being used by the subscription
        /// </summary>
        public int CurrentCoreCount { get; set; }

        /// <summary>
        /// The current number of storage accounts being used by the subscription
        /// </summary>
        public int CurrentStorageAccounts { get; set; }

        /// <summary>
        /// The current number of hosted services being used by the subscription
        /// </summary>
        public int CurrentHostedServices { get; set; }
    }
}