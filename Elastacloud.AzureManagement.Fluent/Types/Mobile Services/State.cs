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
    /// The possible states for a mobile service
    /// </summary>
    public enum State
    {
        /// <summary>
        /// occurs when all of the resources are attached correctly
        /// </summary>
        Healthy,
        /// <summary>
        /// occurs when a database is missing
        /// </summary>
        UnHealthy
    }
}