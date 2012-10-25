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
    /// Structure used to define the operation to perform on a deployment
    /// </summary>
    public enum UpdateDeploymentStatus
    {
        /// <summary>
        /// Stops the deployment
        /// </summary>
        Suspended,
        /// <summary>
        /// Starts the deployment
        /// </summary>
        Running
    }
}