/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

namespace Elastacloud.AzureManagement.Fluent.Services
{
    /// <summary>
    /// Used to add the configuration for Remote Desktop
    /// TODO: Currently this will only need a single Remote Forwarder
    /// </summary>
    public interface IRemoteDesktop
    {
        /// <summary>
        /// Adds a username and password to the configuration 
        /// </summary>
        IServiceCertificate WithUsernameAndPassword(string username, string password);
    }
}