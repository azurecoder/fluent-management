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
    /// Contains the hosted service details 
    /// </summary>
    public class StorageAccount
    {
        /// <summary>
        /// The name of the storage account
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The storage account url http://management ....
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// Returns the primary storage key
        /// </summary>
        public string PrimaryAccessKey { get; set; }
        /// <summary>
        /// Returns the secondary storage key
        /// </summary>
        public string SecondaryAccessKey { get; set; }
        /// <summary>
        /// Gets the correct location of the storage account in the full text format
        /// </summary>
        public string Location { get; set; }
    }
}