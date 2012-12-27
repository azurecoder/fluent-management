/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using Elastacloud.AzureManagement.Fluent.Commands.Services;

namespace Elastacloud.AzureManagement.Fluent.Commands.MobileServices
{
    /// <summary>
    /// Used to a create a deployment for a web or worker role given some specific deployment parameters
    /// </summary>
    internal class MobileServiceCommand : ServiceCommand
    {
        /// <summary>
        /// Constructs a mobile services command
        /// </summary>
        public MobileServiceCommand()
        {
            Accept = "application/xml";
        }
        /// <summary>
        /// The base64 config of the deployment
        /// </summary>
        internal string Config { get; set; }
    }
}