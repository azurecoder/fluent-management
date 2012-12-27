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
    /// The account type that the mobile service uses to log in
    /// </summary>
    public enum AuthAccountType
    {
        /// <summary>
        /// A Microsoft Account used to authenticate 
        /// </summary>
        MicrosoftAccount,
        /// <summary>
        /// A Yahoo account used to authenticate 
        /// </summary>
        Yahoo,
        /// <summary>
        /// A google account used to authenticate
        /// </summary>
        Google,
        /// <summary>
        /// Afacbook account used to authenticate
        /// </summary>
        Facebook,
        /// <summary>
        /// A windows notification service settings Client Secret/Package SID
        /// </summary>
        WNS
    }
}