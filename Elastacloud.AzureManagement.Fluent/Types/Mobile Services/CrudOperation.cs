/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

namespace Elastacloud.AzureManagement.Fluent.Types.MobileServices
{
    /// <summary>
    /// The operations for the data pipeline scripts within mobile services
    /// </summary>
    public enum CrudOperation
    {
        /// <summary>
        /// Used to insert a mobile service table row
        /// </summary>
        Insert,
        /// <summary>
        /// Used to update a mobile service table row
        /// </summary>
        Update,
        /// <summary>
        /// Used to delete a mobile service table row
        /// </summary>
        Delete,
        /// <summary>
        /// Used to read a mobile service table row
        /// </summary>
        Read
    }
}
