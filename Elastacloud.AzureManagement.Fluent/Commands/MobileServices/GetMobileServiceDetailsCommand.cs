/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using Elastacloud.AzureManagement.Fluent.Commands.Parsers;
using Elastacloud.AzureManagement.Fluent.Types.Exceptions;

namespace Elastacloud.AzureManagement.Fluent.Commands.MobileServices
{
    /// <summary>
    /// Used to create a mobile services application
    /// </summary>
    internal class GetMobileServiceDetailsCommand : MobileServiceCommand
    {
        /// <summary>
        /// Constructs a command to create a mobile service
        /// </summary>
        internal GetMobileServiceDetailsCommand(string name)
        {
            OperationId = "mobileservices";
            ServiceType = "services";
            Name = name;
            HttpVerb = HttpVerbGet;
            HttpCommand = "mobileservices/" + name.ToLower();
        }
        /// <summary>
        /// The application key used by the mobile service
        /// </summary>
        public string ApplicationKey { get; private set; }
        /// <summary>
        /// The master key used by the mobile service
        /// </summary>
        public string MasterKey { get; private set; }
        /// <summary>
        /// The application url used by the mobile services client
        /// </summary>
        public string ApplicationUrl { get; private set; }
        /// <summary>
        /// The webspace that the mobile service is bound to
        /// </summary>
        public string Webspace { get; private set; }

        /// <summary>
        /// Used to return the mobile service application details 
        /// </summary>
        protected override void ResponseCallback(System.Net.HttpWebResponse webResponse)
        {
            var dictionary = Parse(webResponse, BaseParser.GetMobileServiceDetailsParser, new GetMobileServiceDetailParser(null));
            if (dictionary != null && dictionary.Count != 5)
            {
                throw new FluentManagementException("Incorrect values returned or service does not exist", ToString());
            }
            ApplicationKey = dictionary["ApplicationKey"];
            ApplicationUrl = dictionary["ApplicationUrl"];
            MasterKey = dictionary["MasterKey"];
            Location = dictionary["Location"];
            Webspace = dictionary["WebSpace"];
            SitAndWait.Set();
        }

        /// <summary>
        /// Returns the string name of the command
        /// </summary>
        public override string ToString()
        {
            return "GetMobileServiceDetailsCommand";
        }
    }
}