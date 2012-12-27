/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System;
using System.Collections.Generic;
using Elastacloud.AzureManagement.Fluent.Commands.Parsers;
using Elastacloud.AzureManagement.Fluent.Types.Exceptions;
using Elastacloud.AzureManagement.Fluent.Types.MobileServices;

namespace Elastacloud.AzureManagement.Fluent.Commands.MobileServices
{
    /// <summary>
    /// Used to create a mobile services application
    /// </summary>
    internal class UpdateMobileServiceSettingsCommand : MobileServiceCommand
    {
        /// <summary>
        /// Constructs a command to create a mobile service
        /// </summary>
        internal UpdateMobileServiceSettingsCommand(string serviceName, string path, string config, string overrideVerb = HttpVerbPut)
        {
            OperationId = "mobileservices";
            ServiceType = "services";
            Name = serviceName;
            HttpVerb = overrideVerb;
            Config = config;
            ContentType = "application/json"; 
            HttpCommand = string.Format("mobileservices/{0}/{1}", serviceName.ToLower(), path);
        }

        /// <summary>
        /// Returns the JSON formatted configuration
        /// </summary>
        protected override string CreatePayload()
        {
            return Config;
        }

        /// <summary>
        /// Returns the string name of the command
        /// </summary>
        public override string ToString()
        {
            return "UdpateMobileServiceSettingsCommand";
        }
    }
}