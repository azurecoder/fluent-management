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
using System.Net;
using System.Xml.Linq;

namespace Elastacloud.AzureManagement.Fluent.Commands.Parsers
{
    /// <summary>
    /// Base class used to parse all XML responses from the fabric
    /// </summary>
    internal abstract class BaseParser
    {
        /// <summary>
        /// The command response dynamic object containing the parsed entity
        /// </summary>
        internal dynamic CommandResponse = new object();

        /// <summary>
        /// The Xml document returned and being parsed
        /// </summary>
        protected XDocument Document;

        /// <summary>
        /// To construct the BaseParser
        /// </summary>
        /// <param name="response">The XmlDocument used to construct the parser</param>
        internal BaseParser(XDocument response)
        {
            Document = response;
        }

        /// <summary>
        /// Used in the chain of responsibility pattern to determine which class should be responsible for said return
        /// </summary>
        internal abstract string RootElement { get; }


        /// <summary>
        /// Overriden in derived classes with the schema value of the operation and response
        /// </summary>
        protected abstract XNamespace GetSchema();

        /// <summary>
        /// Gets an instance of the BaseParser type to use and parse response - implementation of CoR pattern
        /// </summary>
        public static BaseParser GetInstance(HttpWebResponse response, string rootVerb, BaseParser parser = null
            /* used when there are more than one parser with the same root node - have to change pattern not ideal */)
        {
            XDocument document;
            try
            {
                document = XDocument.Load(response.GetResponseStream());
            }
            catch (Exception)
            {
                throw new ApplicationException("XML document not present in response stream");
            }
            // parser needs to be checked if it's a duplicate root element - discard the CoR check - yuck!
            if (parser != null)
            {
                parser.Document = document;
                return parser;
            }

            var parserList = new List<BaseParser>
                                 {
                                     new AddNewSqlAzureServerParser(document),
                                     new GetHostedServiceListParser(document),
                                     new ListStorageAccountsParser(document),
                                     new GetSubscriptionParser(document),
                                     new GetSubscriberLocationsParser(document),
                                     new GetAggregateDeploymentStatusParser(document),
                                     new GetRoleStatusParser(document),
                                     new GetHostedServicePropertiesParser(document)
                                 };
            foreach (BaseParser baseParser in parserList)
            {
                if (baseParser.RootElement == rootVerb)
                    return baseParser;
            }
            throw new ApplicationException("Unknown response - parser not available");
        }

        /// <summary>
        /// Must implement in a derived class since this needs Xml specific parse code
        /// </summary>
        internal abstract void Parse();

        #region Root Verbs

        /// <summary>
        /// The root element for the mobile services table list
        /// </summary>
        public const string ListMobileServiceTablesParser = "Tables";
        /// <summary>
        /// The element for the AddNewSqlAzureServerParser response
        /// </summary>
        public const string AddNewSqlAzureServerParser = "ServerName";

        /// <summary>
        /// The element for the GetHostedServiceListParser response
        /// </summary>
        public const string GetHostedServiceListParser = "HostedServices";

        /// <summary>
        /// The element for the ListStorageAccountsParser response
        /// </summary>
        public const string ListStorageAccountsParser = "StorageServices";

        /// <summary>
        /// The element for the GetSubscriptionParser response
        /// </summary>
        public const string GetSubscriptionParser = "Subscription";

        /// <summary>
        /// The element for the GetSubscriberLocationsParser response
        /// </summary>
        public const string GetSubscriberLocationsParser = "Locations";

        /// <summary>
        /// The element for the GetAggregateDeploymentsParser response
        /// </summary>
        public const string GetAggregateDeploymentsParser = "Deployment";
        /// <summary>
        /// The element for the GetRoleStatusParser response
        /// </summary>
        public const string GetRoleStatusParser = "Deployment";
        /// <summary>
        /// The element for the GetDeploymentRoleNamesParser response
        /// </summary>
        public const string GetDeploymentRoleNamesParser = "Deployment";
        /// <summary>
        /// The element for the GetDeploymentConfigurationParser response
        /// </summary>
        public const string GetDeploymentConfigurationParser = "Deployment";
        /// <summary>
        /// Used to get the cloud service properties
        /// </summary>
        public const string GetCloudServicePropertiesParser = "HostedService";
        /// <summary>
        /// Gets the details for the mobile services application
        /// </summary>
        public const string GetMobileServiceDetailsParser = "ServiceResource";
        /// <summary>
        /// Gets the details for the mobile services application
        /// </summary>
        public const string GetMobileServiceTablePermissionsParser = "Permissions";
        /// <summary>
        /// Gets the mobile services resource parser 
        /// </summary>
        public const string GetMobileServiceResourcesParser = "Application";
        /// <summary>
        /// The root command for Windows Azure websites
        /// </summary>
        public const string WebsiteListParser = "WebSpaces";
        /// <summary>
        /// The root element returned when requesting a particular webspace
        /// </summary>
        public const string WebsiteParser = "Sites";
        /// <summary>
        /// The config of the sit parser tag
        /// </summary>
        public const string WebsiteConfigParser = "SiteConfig";
        /// <summary>
        /// The root element for the server farm for WAWS
        /// </summary>
        public const string WebsiteServerFarm = "ServerFarm";
        /// <summary>
        /// The Xml for the check cloud service name available response 
        /// </summary>
        public const string CloudServiceAvailable = "AvailabilityResponse";
        
        /// <summary>
        /// The schema for the Sql azure manmagement request
        /// </summary>
        public const string SqlAzureSchema = "http://schemas.microsoft.com/sqlazure/2010/12/";
        /// <summary>
        /// The generic windows azure parser response address
        /// </summary>
        public const string WindowsAzureSchema = "http://schemas.microsoft.com/windowsazure";
        /// <summary>
        /// The schema used by WAMS
        /// </summary>
        public const string MobileServicesSchema = "http://schemas.microsoft.com/windowsazure/mobileservices";
        /// <summary>
        /// The Service Management schema address
        /// </summary>
        public const string ServiceManagementSchema =
            "http://schemas.microsoft.com/ServiceHosting/2008/10/ServiceConfiguration";

        #endregion
    }
}