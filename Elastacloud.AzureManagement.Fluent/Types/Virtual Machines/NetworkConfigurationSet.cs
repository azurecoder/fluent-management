/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System.Collections.Generic;
using System.Xml.Linq;
using Elastacloud.AzureManagement.Fluent.Helpers;
using FSharp.Data.Runtime;

namespace Elastacloud.AzureManagement.Fluent.Types.VirtualMachines
{
    /// <summary>
    /// Is the network configuration set for the vm instance
    /// </summary>
    public class NetworkConfigurationSet : ConfigurationSet
    {
        /// <summary>
        /// Contains the InputEndpoints for the network configuration set
        /// </summary>
        public InputEndpoints InputEndpoints { get; set; }
        /// <summary>
        /// The name of a valid subnet associated with a vnet
        /// </summary>
        public string SubnetName { get; set; }
        /// <summary>
        /// The set of permit deny rules available between subnets
        /// </summary>
        public EndpointAcl EndpointAcl { get; set; }

        #region Implementation of ICustomXmlSerializer

        /// <summary>
        /// Gets the Xml tree for the custom serialiser
        /// </summary>
        /// <returns>An XElement </returns>
        public override XElement GetXmlTree()
        {
            var element = new XElement(Namespaces.NsWindowsAzure + "ConfigurationSet", InputEndpoints.GetXmlTree());
            element.Add(new XElement(Namespaces.NsWindowsAzure + "ConfigurationSetType", ConfigurationSetType.ToString()));
            if (SubnetName != null)
            {
                element.Add(new XElement(Namespaces.NsWindowsAzure + "SubnetNames", new XElement(Namespaces.NsWindowsAzure + "SubnetName", SubnetName)));
            }
            if (EndpointAcl != null)
            {
                element.Add(EndpointAcl.GetXmlTree());
            }
            return element;
        }

        #endregion

        #region Overrides of ConfigurationSet

        /// <summary>
        /// Used to set the type of configuration set used with this vm instance
        /// </summary>
        public override ConfigurationSetType ConfigurationSetType
        {
            get { return ConfigurationSetType.NetworkConfiguration; }
        }

        #endregion
    }
}