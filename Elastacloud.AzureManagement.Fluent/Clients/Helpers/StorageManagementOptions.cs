/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

namespace Elastacloud.AzureManagement.Fluent.Clients.Helpers
{
    /// <summary>
    /// The storage details that the account will enable
    /// </summary>
    public class StorageManagementOptions
    {
        /// <summary>
        /// Whether or not the storage supported is secondary read only as well 
        /// </summary>
        public bool SecondaryReadOnly { get; set; }
        /// <summary>
        /// The type of storage account whether GRS, LRS or premium
        /// </summary>
        public StorageType StorageType { get; set; }

        /// <summary>
        /// Defaults enabled for the 
        /// </summary>
        public static StorageManagementOptions GetDefaultOptions
        {
            get
            {
                return new StorageManagementOptions()
                {
                    SecondaryReadOnly = false,
                    StorageType = StorageType.Standard_LRS
                };
            }   
        }
    }
}