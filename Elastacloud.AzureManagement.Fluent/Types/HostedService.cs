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
    /// Contains the hosted service details 
    /// </summary>
    public class HostedService
    {
        /// <summary>
        /// The name of the hosted service
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The hosted service url *.cloudapp.net
        /// </summary>
        public string Url { get; set; }
    }
}