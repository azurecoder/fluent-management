/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System.Net;
using Elastacloud.AzureManagement.Fluent.Commands.Parsers;
using Elastacloud.AzureManagement.Fluent.Commands.Services;

namespace Elastacloud.AzureManagement.Fluent.Commands.SqlAzure
{
    /// <summary>
    /// Used to delete a SQL Azure instance given the name
    /// </summary>
    internal class DeleteSqlFirewallRuleCommand : ServiceCommand
    {
        /// <summary>
        /// the name of the management endpoint
        /// </summary>
        public const string SqlAzureManagementEndpoint = "https://management.database.windows.net:8443";

        /// <summary>
        /// The schema used in the return Xml (if any)
        /// </summary>
        public const string SqlAzureSchema = BaseParser.SqlAzureSchema;

        /// <summary>
        /// Used to construct the delete command
        /// </summary>
        /// <param name="serverName">The name of the Sql Azure Server instance</param>
        internal DeleteSqlFirewallRuleCommand(string serverName, string ruleName)
        {
            HttpVerb = HttpVerbDelete;
            ServiceType = "servers";
            BaseRequestUri = SqlAzureManagementEndpoint;
            AdditionalHeaders["x-ms-version"] = "1.0";
            // set the servername to send with the URL
            OperationId = SqlAzureServerName = serverName;
            HttpCommand = "firewallrules/" + ruleName;
        }

        /// <summary>
        /// The name of the Sql Azure Server instance
        /// </summary>
        public string SqlAzureServerName { get; set; }

        /// <summary>
        /// Used to get the response and move on since there is no async activity with the Db
        /// </summary>
        protected override void ResponseCallback(HttpWebResponse webResponse)
        {
            // Important to note that this command only operates synchronously so we don't want to block here!
            SitAndWait.Set();
        }
    }
}