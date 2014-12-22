/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System.Xml.Linq;
using Elastacloud.AzureManagement.Fluent.Commands.Parsers;

namespace Elastacloud.AzureManagement.Fluent.Helpers
{
    /// <summary>
    /// A static class which contains all of the namespace references that are used in the XML requests to Windows Azure
    /// </summary>
    public static class Namespaces
    {
        /// <summary>
        /// The Service defintion namespace text
        /// </summary>
        public static string ServiceDefinition = "http://schemas.microsoft.com/ServiceHosting/2008/10/ServiceDefinition";
        /// <summary>
        /// The BaseUri used to manage Sql Azure
        /// </summary>
        public static string SqlServiceManagement = "https://management.core.windows.net:8443";

        /// <summary>
        /// An XNamespace which container the service management schema
        /// </summary>
        public static XNamespace NsServiceManagement = XNamespace.Get(BaseParser.ServiceManagementSchema);

        /// <summary>
        /// An XNamespace which container the generic windows azure schema
        /// </summary>
        public static XNamespace NsWindowsAzure = XNamespace.Get(BaseParser.WindowsAzureSchema);
        /// <summary>
        /// The Windows Azure Mobile Services namespace
        /// </summary>
        public static XNamespace NsWindowsAzureMobileServices = XNamespace.Get(BaseParser.MobileServicesSchema);
        /// <summary>
        /// An XNamespace which container the service management schema
        /// </summary>
        public static XNamespace NsServiceDefinition = XNamespace.Get(ServiceDefinition);
        /// <summary>
        /// Used to manage Sql Azure 
        /// </summary>
        public static XNamespace NsSqlServiceManagement = XNamespace.Get(SqlServiceManagement);
        /// <summary>
        /// The networking config namespace schema 
        /// </summary>
        public static XNamespace NetworkingConfig =
            "http://schemas.microsoft.com/ServiceHosting/2011/07/NetworkConfiguration";

    }
}