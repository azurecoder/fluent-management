/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System;
using System.Net;
using System.Xml.Linq;
using Elastacloud.AzureManagement.Fluent.Commands.Services;
using Elastacloud.AzureManagement.Fluent.Helpers;

namespace Elastacloud.AzureManagement.Fluent.Commands.SqlAzure
{
    internal class AddNewFirewallRuleCommand : ServiceCommand, ISqlAzureFirewallRule
    {
        public const string SqlAzureSchema = "http://schemas.microsoft.com/sqlazure/2010/12/";
        public const string SqlAzureManagementEndpoint = "https://management.database.windows.net:8443";

        internal AddNewFirewallRuleCommand(string rulename, string iplow, string iphigh)
        {
            HttpVerb = "PUT";
            ServiceType = "servers";
            BaseRequestUri = SqlAzureManagementEndpoint;
            AdditionalHeaders["x-ms-version"] = "1.0";
            SqlAzureClientIpAddressLow = iplow;
            SqlAzureClientIpAddressHigh = iphigh;
            SqlAzureRuleName = rulename;
        }

        #region ISqlAzureFirewallRule Members

        public string SqlAzureServerName { get; set; }
        public string SqlAzureRuleName { get; set; }
        public string SqlAzureClientIpAddressHigh { get; set; }
        public string SqlAzureClientIpAddressLow { get; set; }

        // https://management.database.windows.net:8443/<subscription-id>/servers/<servername>/firewallrules/<rulename>

        public void ConfigureFirewallCommand(string server)
        {
            OperationId = SqlAzureServerName = server;
            HttpCommand = String.Format("firewallrules/{0}", SqlAzureRuleName);
        }

        #endregion

        protected override string CreatePayload()
        {
            XNamespace ns = SqlAzureSchema;
            var doc = new XDocument(
                new XDeclaration("1.0", "utf-8", ""),
                new XElement(ns + "FirewallRule",
                             new XElement(ns + "StartIpAddress", SqlAzureClientIpAddressLow),
                             new XElement(ns + "EndIpAddress", SqlAzureClientIpAddressHigh)));
            return doc.ToStringFullXmlDeclaration();
        }

        protected override void ResponseCallback(HttpWebResponse webResponse)
        {
            SitAndWait.Set();
        }
    }
}