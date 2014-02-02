/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/


using System.Security.Cryptography.X509Certificates;

namespace Elastacloud.AzureManagement.Fluent.Clients.Interfaces
{
    /// <summary>
    /// Used to manipulate images intra-subscription or between subscriptions
    /// </summary>
    public interface IImageManagementClient
    {
        /// <summary>
        /// Used to copy or register an image from one subscription to another
        /// </summary>
        void CopyAndRegisterImageInNewSubscription(string accountName, string accountKey, string containerName,
            string imageName, string imageUri);
    }
}
