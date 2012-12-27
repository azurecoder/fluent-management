/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System;
using System.Xml.Linq;
using Elastacloud.AzureManagement.Fluent.Helpers;
using Elastacloud.AzureManagement.Fluent.Types.MobileServices;

namespace Elastacloud.AzureManagement.Fluent.Commands.MobileServices
{
    /// <summary>
    /// Used to create a mobile services application
    /// </summary>
    internal class CreateMobileServiceTableScriptCommand : MobileServiceCommand
    {
        /// <summary>
        /// Constructs a command to create a mobile service
        /// </summary>
        internal CreateMobileServiceTableScriptCommand(string serviceName, string tableName, CrudOperation operation, string config)
        {
            OperationId = "mobileservices";
            ServiceType = "services";
            Name = serviceName;
            TableName = tableName;
            Config = config;
            HttpVerb = HttpVerbPut;
            ContentType = "text/plain";
            HttpCommand = String.Format("mobileservices/{0}/tables/{1}/scripts/{2}/code", serviceName.ToLower(), tableName.ToLower(), operation.ToString().ToLower());
        }

        /// <summary>
        /// The name of the WAMS table
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// The crud operation the command is being undertaken on
        /// </summary>
        public CrudOperation Operation { get; set; }

        /// <summary>
        /// Returns a JSON payload to setup the table
        /// </summary>
        protected override string CreatePayload()
        {
            return Config;
        }

        public override void Execute()
        {
            base.Execute();
        }

        /// <summary>
        /// Returns the string name of the command
        /// </summary>
        public override string ToString()
        {
            return "CreateMobileServiceTableScriptCommand";
        }
    }
}