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
    /// The roles used by Mobile Services to control access to scripts
    /// </summary>
    public enum Roles
    {
        /// <summary>
        /// Any recognised user
        /// </summary>
        User,
        /// <summary>
        /// The application being used to access the mobile services script
        /// </summary>
        Application,
        /// <summary>
        /// Also the administrator of the mobile service
        /// </summary>
        Admin,
        /// <summary>
        /// Any anonymous member 
        /// </summary>
        Public
    }
}
