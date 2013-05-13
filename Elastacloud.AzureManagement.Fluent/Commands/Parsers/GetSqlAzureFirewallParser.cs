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
using System.Linq;
using System.Xml.Linq;
using Elastacloud.AzureManagement.Fluent.Types;

namespace Elastacloud.AzureManagement.Fluent.Commands.Parsers
{
    internal class GetSqlAzureFirewallParser : BaseParser
    {
        public GetSqlAzureFirewallParser(XDocument response)
            : base(response)
        {
            CommandResponse = new List<SqlFirewallRule>();
        }

        /*
         * <FirewallRules xmlns="http://schemas.microsoft.com/sqlazure/2010/12/">
              <FirewallRule>
                <Name>Test_Firewall_Rule</Name>
                <StartIpAddress>10.20.30.0</StartIpAddress>
                <EndIpAddress>10.20.30.100</EndIpAddress>
              </FirewallRule>
            </FirewallRules>
*/
        internal override void Parse()
        {
            IEnumerable<XElement> rootElements = Document.Element(GetSchema() + RootElement)
                .Elements(GetSchema() + "FirewallRule");

            rootElements.ToList().ForEach(rule => CommandResponse.Add(
                new SqlFirewallRule()
                    {
                        RuleName = (string) rule.Element(GetSchema() + "Name"),
                        IpAddressHigh = (string) rule.Element(GetSchema() + "StartIpAddress"),
                        IpAddressLow = (string) rule.Element(GetSchema() + "EndIpAddress")
                    }));
        }

        #region Overrides of BaseParser

        internal override string RootElement
        {
            get { return "FirewallRules"; }
        }

        protected override XNamespace GetSchema()
        {
            return XNamespace.Get(SqlAzureSchema);
        }

        #endregion
    }
}