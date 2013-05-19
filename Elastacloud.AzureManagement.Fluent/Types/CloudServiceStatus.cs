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
    /// Used to denote the status of the cloud service
    /// </summary>
    public enum CloudServiceStatus
    {
        /// <summary>
        /// The cloud service has been created
        /// </summary>
        Created,
        /// <summary>
        /// The cloud service is currently being created
        /// </summary>
        Creating,
        /// <summary>
        /// The cloud service has been deleted
        /// </summary>
        Deleted,
        /// <summary>
        /// The cloud service is currently deleting
        /// </summary>
        Deleting,
        /// <summary>
        /// Something is changing or updating with the cloud service
        /// </summary>
        Changing,
        /// <summary>
        /// The Dns is resolving on the cloud service URI
        /// </summary>
        ResolvingDns
    }
}