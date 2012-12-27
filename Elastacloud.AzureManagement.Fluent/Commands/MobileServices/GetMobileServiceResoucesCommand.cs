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
using Elastacloud.AzureManagement.Fluent.Types.MobileServices;

namespace Elastacloud.AzureManagement.Fluent.Commands.MobileServices
{
    /// <summary>
    /// Used to create a mobile services application
    /// </summary>
    internal class GetMobileServiceResourcesCommand : MobileServiceCommand
    {
        /// <summary>
        /// Constructs a command to create a mobile service
        /// </summary>
        internal GetMobileServiceResourcesCommand(string name)
        {
            ServiceType = "applications";
            Name = name;
            HttpVerb = HttpVerbGet;
            HttpCommand = Name + "mobileservice";
        }
        /// <summary>
        /// The name of the database as far as mobile services is concerned
        /// </summary>
        public string DatabaseName { get; private set; }
        /// <summary>
        /// The name of the Sql Azure server as far as mobile services is concerned
        /// </summary>
        public string ServerName { get; private set; }
        /// <summary>
        /// The name of the moniker for the database for mobile services
        /// </summary>
        public string MobileServiceDatabaseName { get; private set; }
        /// <summary>
        /// The name of the moniker for the sql azure server instance  for mobile services
        /// </summary>
        public string MobileServiceServerName { get; private set; }
        /// <summary>
        /// The state of the mobile service
        /// </summary>
        public State State { get; private set; }


        /// <summary>
        /// Used to return the mobile service application details 
        /// </summary>
        protected override void ResponseCallback(System.Net.HttpWebResponse webResponse)
        {
            var dictionary = Parse(webResponse, BaseParser.GetMobileServiceResourcesParser, new GetMobileServiceResourceParser(null));
            if (dictionary != null && dictionary.Count != 6)
            {
                throw new FluentManagementException("Incorrect values returned or service does not exist", ToString());
            }
            DatabaseName = dictionary["DatabaseName"];
            ServerName = dictionary["ServerName"];
            MobileServiceServerName = dictionary["MobileServiceServerName"];
            MobileServiceDatabaseName = dictionary["MobileServiceDatabaseName"];
            Description = dictionary["Description"];
            State = (State)Enum.Parse(typeof(State), dictionary["State"]);
            SitAndWait.Set();
        }

        /// <summary>
        /// Returns the string name of the command
        /// </summary>
        public override string ToString()
        {
            return "GetMobileServiceResourcesCommand";
        }
    }
}