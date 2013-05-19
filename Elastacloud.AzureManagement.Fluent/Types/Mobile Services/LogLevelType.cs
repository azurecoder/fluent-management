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
    /// Used to describe the type of event logged in the mobile service logs
    /// </summary>
    public enum LogLevelType
    {
        /// <summary>
        /// A error entry in the log
        /// </summary>
        Error,
        /// <summary>
        /// An info entry in the log
        /// </summary>
        Information,
        /// <summary>
        /// A warning entry in the log
        /// </summary>
        Warning
    }
}