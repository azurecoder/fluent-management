/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;
using Elastacloud.AzureManagement.Fluent.Helpers;
using FSharp.Data.Runtime;

namespace Elastacloud.AzureManagement.Fluent.Types.VirtualMachines
{
    /// <summary>
    /// Used to permit or dey access to the specfic subnet
    /// </summary>
    public enum ActionAcl
    {
        permit,
        deny
    }
    /// <summary>
    /// Adds an Endpoint to the Virtual Machine which in effect creates a firewall rule
    /// </summary>
    public class EndpointAclRule : ICustomXmlSerializer
    {
        /*    <EndpointACL>
                <Rules>
                  <Rule>
                    <Order>priority-of-the-rule</Order>
                    <Action>permit-rule</Action>
                    <RemoteSubnet>subnet-of-the-rule</RemoteSubnet>
                    <Description>description-of-the-rule</Description>
                  </Rule>
                </Rules>
              </EndpointACL>
*/

        /// <summary>
        /// The order that thet rules runs in 
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Whether to permit or deny the remote subnet from accessing
        /// </summary>
        public ActionAcl PermitDeny { get; set; }

        /// <summary>
        /// The CIDR notation of the remote subnet 
        /// </summary>
        public string RemoteSubnet { get; set; }

        #region Implementation of ICustomXmlSerializer

        /// <summary>
        /// Gets the Xml tree for the custom serialiser
        /// </summary>
        /// <returns>An XElement </returns>
        public XElement GetXmlTree()
        {
            var element = new XElement(Namespaces.NsWindowsAzure + "Rule",
                                       new XElement(Namespaces.NsWindowsAzure + "Order", Order.ToString(CultureInfo.InvariantCulture)),
                                       new XElement(Namespaces.NsWindowsAzure + "Action", PermitDeny),
                                       new XElement(Namespaces.NsWindowsAzure + "RemoteSubnet", RemoteSubnet));
            return element;
        }

        #endregion
    }

    public class EndpointAclRules : ICustomXmlSerializer
    {
        public EndpointAclRules(List<EndpointAclRule> rules)
        {
            Rules = rules;
        }
        public List<EndpointAclRule> Rules { get; set; }

        /// <summary>
        /// Gets the Xml tree for the custom serialiser
        /// </summary>
        /// <returns>An XElement </returns>
        public XElement GetXmlTree()
        {
            var element = new XElement(Namespaces.NsWindowsAzure + "Rules");
            foreach (var rule in Rules)
            {
                element.Add(rule);
            }
            return element;
        }
    }

    public class EndpointAcl : ICustomXmlSerializer
    {
        public EndpointAcl(EndpointAclRules rules)
        {
            Rules = rules;
        }

        public EndpointAclRules Rules { get; set; }
        /// <summary>
        /// Gets the Xml tree for the custom serialiser
        /// </summary>
        /// <returns>An XElement </returns>
        public XElement GetXmlTree()
        {
            return new XElement(Namespaces.NsWindowsAzure + "EndpointACL",
                Rules.GetXmlTree());
        }
    }
}