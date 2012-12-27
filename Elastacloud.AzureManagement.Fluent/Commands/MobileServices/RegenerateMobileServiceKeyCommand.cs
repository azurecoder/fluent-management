/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System;
using Elastacloud.AzureManagement.Fluent.Types.MobileServices;

namespace Elastacloud.AzureManagement.Fluent.Commands.MobileServices
{
    /// <summary>
    /// Used to create a mobile services application
    /// </summary>
    internal class RegenerateMobileServiceKeyCommand : MobileServiceCommand
    {
        /// <summary>
        /// Constructs a command to create a mobile service
        /// </summary>
        internal RegenerateMobileServiceKeyCommand(string serviceName, KeyType type = KeyType.Application)
        {
            OperationId = "mobileservices";
            ServiceType = "services";
            Name = serviceName;
            HttpVerb = HttpVerbPost;
            KeyType = type;
            HttpCommand = String.Format("mobileservices/{0}/regenerateKey?type={1}", serviceName.ToLower(), type.ToString().ToLower());
        }
        /// <summary>
        /// the type of key needed to be regenerated
        /// </summary>
        public KeyType KeyType { get; set; }
        /// <summary>
        /// Returns the string name of the command
        /// </summary>
        public override string ToString()
        {
            return "RegenerateMobileServiceKeyCommand";
        }
    }
}