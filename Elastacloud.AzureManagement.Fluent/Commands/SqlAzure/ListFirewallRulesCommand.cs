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
using Elastacloud.AzureManagement.Fluent.Commands.Parsers;
using Elastacloud.AzureManagement.Fluent.Commands.Services;
using Elastacloud.AzureManagement.Fluent.Helpers;
using Elastacloud.AzureManagement.Fluent.Types;

namespace Elastacloud.AzureManagement.Fluent.Commands.SqlAzure
{
    internal class ListFirewallRulesCommand : ServiceCommand
    {
        public const string SqlAzureSchema = "http://schemas.microsoft.com/sqlazure/2010/12/";
        public const string SqlAzureManagementEndpoint = "https://management.database.windows.net:8443";

        internal ListFirewallRulesCommand(string serverName)
        {
            HttpVerb = "GET";
            ServiceType = "servers";
            OperationId = serverName;
            HttpCommand = "firewallrules";
            BaseRequestUri = SqlAzureManagementEndpoint;
            AdditionalHeaders["x-ms-version"] = "1.0";
        }

        public List<SqlFirewallRule> FirewallRules { get; set; }

        protected override void ResponseCallback(HttpWebResponse webResponse)
        {
            FirewallRules = Parse(webResponse, "FirewallRules", new GetSqlAzureFirewallParser(null));
            SitAndWait.Set();
        }
    }
}