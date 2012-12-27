/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System;
using Elastacloud.AzureManagement.Fluent.Commands.Parsers;
using Elastacloud.AzureManagement.Fluent.Types.Exceptions;

namespace Elastacloud.AzureManagement.Fluent.Commands.MobileServices
{
    /// <summary>
    /// Used to create a mobile services application
    /// </summary>
    internal class GetMobileServiceTablePermissionCommand : MobileServiceCommand
    {
        /// <summary>
        /// Constructs a command to create a mobile service
        /// </summary>
        internal GetMobileServiceTablePermissionCommand(string serviceName, string tableName)
        {
            OperationId = "mobileservices";
            ServiceType = "services";
            Name = serviceName;
            TableName = tableName;
            HttpVerb = HttpVerbGet;
            HttpCommand = string.Format("mobileservices/{0}/tables/{1}/permissions", serviceName.ToLower(), tableName.ToLower());
        }
        /// <summary>
        /// The name of the mobile services table
        /// </summary>
        protected string TableName { get; set; }

        /// <summary>
        /// Permission used for an insert script
        /// </summary>
        public Types.MobileServices.Roles InsertPermission { get; set; }
        /// <summary>
        /// Permission used for a read script
        /// </summary>
        public Types.MobileServices.Roles ReadPermission { get; set; }
        /// <summary>
        /// Permission used for an update script
        /// </summary>
        public Types.MobileServices.Roles UpdatePermission { get; set; }
        /// <summary>
        /// Permission used for a delete script
        /// </summary>
        public Types.MobileServices.Roles DeletePermission { get; set; }

        /// <summary>
        /// Used to return the mobile service application details 
        /// </summary>
        protected override void ResponseCallback(System.Net.HttpWebResponse webResponse)
        {
            var dictionary = Parse(webResponse, BaseParser.GetMobileServiceTablePermissionsParser, new GetMobileServiceTablePermissionsParser(null));
            if (dictionary != null && dictionary.Count != 4)
            {
                throw new FluentManagementException("Incorrect values returned or service does not exist", ToString());
            }
            InsertPermission = (Types.MobileServices.Roles)Enum.Parse(typeof(Types.MobileServices.Roles), dictionary["Insert"]);
            ReadPermission = (Types.MobileServices.Roles)Enum.Parse(typeof(Types.MobileServices.Roles), dictionary["Read"]);
            UpdatePermission = (Types.MobileServices.Roles)Enum.Parse(typeof(Types.MobileServices.Roles), dictionary["Update"]);
            DeletePermission = (Types.MobileServices.Roles)Enum.Parse(typeof(Types.MobileServices.Roles), dictionary["Delete"]);
            
            SitAndWait.Set();
        }

        /// <summary>
        /// Returns the string name of the command
        /// </summary>
        public override string ToString()
        {
            return "GetMobileServiceTablePermissionsCommand";
        }
    }
}