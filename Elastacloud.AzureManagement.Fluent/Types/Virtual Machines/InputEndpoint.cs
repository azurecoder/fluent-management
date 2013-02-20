/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System.Globalization;
using System.Xml.Linq;
using Elastacloud.AzureManagement.Fluent.Helpers;

namespace Elastacloud.AzureManagement.Fluent.Types.VirtualMachines
{
    /// <summary>
    /// Adds an Endpoint to the Virtual Machine which in effect creates a firewall rule
    /// </summary>
    public class InputEndpoint : ICustomXmlSerializer
    {
        /*<InputEndpoint>
        <EnableDirectServerReturn>true|false</EnableDirectServerReturn>                
        <LoadBalancedEndpointSetName></LoadBalancedEndpointSetName>
        <LocalPort>local-port-number</LocalPort>
        <Name>endpoint-name</Name>
        <Port>external-port-number</Port>
        <LoadBalancerProbe>
            <Path>relative-path-that-contains-status</Path>
            <Port>port-to-use-for-status</Port>
            <Protocol>TCP|UDP</Protocol>
        </LoadBalancerProbe>
        <Protocol>TCP|UDP</Protocol>                    
      </InputEndpoint>*/

        /// <summary>
        /// The local port used to connect to the virtual machine instance
        /// </summary>
        public int LocalPort { get; set; }

        /// <summary>
        /// The "friendly name" of the endpoint for the firewall rule
        /// </summary>
        public string EndpointName { get; set; }

        /// <summary>
        /// The public port that will be used by the connecting user 
        /// </summary>
        public int? Port { get; set; }

        // TODO: Build the load balancer port details here
        /// <summary>
        /// The protocol that is used when setting the inbound port rule on the firewall can be TCP or UDP
        /// </summary>
        public Protocol Protocol { get; set; }
        /// <summary>
        /// The virtual Ip address
        /// </summary>
        public string Vip { get; set; }

        #region Templates

        /// <summary>
        /// The default settings for remote desktop
        /// </summary>
        /// <returns>An InputEndpoint with TCP over 3389 Local/External port</returns>
        public static InputEndpoint GetDefaultRemoteDesktopSettings()
        {
            return new InputEndpoint
                       {
                           EndpointName = "RDP",
                           Protocol = Protocol.TCP,
                           LocalPort = 3389,
                           //Port = 3389
                       };
        }

        /// <summary>
        /// The default settings for the SQL Server virtual machine
        /// </summary>
        /// <returns>An endpoint with the default 1433 Local/External port open</returns>
        public static InputEndpoint GetDefaultSqlServerSettings()
        {
            return new InputEndpoint
                       {
                           EndpointName = "SQL_Server",
                           Protocol = Protocol.TCP,
                           LocalPort = 1433,
                           Port = 1433
                       };
        }

        #endregion

        #region Implementation of ICustomXmlSerializer

        /// <summary>
        /// Gets the Xml tree for the custom serialiser
        /// </summary>
        /// <returns>An XElement </returns>
        public XElement GetXmlTree()
        {
            var element = new XElement(Namespaces.NsWindowsAzure + "InputEndpoint",
                                       new XElement(Namespaces.NsWindowsAzure + "LocalPort", LocalPort.ToString(CultureInfo.InvariantCulture)),
                                       new XElement(Namespaces.NsWindowsAzure + "Name", EndpointName.ToString(CultureInfo.InvariantCulture)));
            if (Port.HasValue)
                element.Add(new XElement(Namespaces.NsWindowsAzure + "Port", Port.Value.ToString(CultureInfo.InvariantCulture)));
            element.Add(new XElement(Namespaces.NsWindowsAzure + "Protocol", content: Protocol.ToString().ToLower()));
            return element;
        }

        #endregion
    }
}