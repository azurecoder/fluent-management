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
    /// The Action that will take place on the service type
    /// </summary>
    public enum ActionType
    {
        /// <summary>
        /// Create a particule service type
        /// </summary>
        Create,

        /// <summary>
        /// Update or use a particular service type
        /// </summary>
        Update,

        /// <summary>
        /// Delete a particular service type
        /// </summary>
        Delete
    };
}