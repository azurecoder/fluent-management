/************************************************************************************************************
 * This software is distributed under a GNU Lesser License by Elastacloud Limited and it is free to         *
 * modify and distribute providing the terms of the license are followed. From the root of the source the   *
 * license can be found in /Resources/license.txt                                                           * 
 *                                                                                                          *
 * Web at: www.elastacloud.com                                                                              *
 * Email: info@elastacloud.com                                                                              *
 ************************************************************************************************************/

using System;
using System.Globalization;
using System.Xml.Linq;
using Elastacloud.AzureManagement.Fluent.Helpers;

namespace Elastacloud.AzureManagement.Fluent.Types.VirtualMachines
{
    /// <summary>
    /// Used to represent a configuration for a persistent vm installation of a Windows image
    /// </summary>
    public class WindowsConfigurationSet : ConfigurationSet, ICustomXmlSerializer
    {
        /// <summary>
        /// The name of the machine - given the normal constraints and character limits
        /// </summary>
        public string ComputerName { get; set; }

        /// <summary>
        /// The admin password of the vm instance - the default username is Administrator
        /// </summary>
        public string AdminPassword { get; set; }

        /// <summary>
        /// Whether the password should be reset on the first login - the default for this is false
        /// !!! This is now deprecated 
        /// </summary>
        [Obsolete("This has now been removed from the VM definition")]
        public bool ResetPasswordOnFirstLogon { get; set; }

        /// <summary>
        /// Enables the setting of automatic updates on the windows machine
        /// </summary>
        public bool EnableAutomaticUpdate { get; set; }

        /// <summary>
        /// Represents a valid timezone HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Time Zones 
        /// Will be "GMT Standard Time" or "GMT Daylight Time"
        /// </summary>
        public string TimeZone { get; set; }

        #region Implementation of ICustomXmlSerializer

        /// <summary>
        /// Gets the Xml tree for the custom serialiser
        /// </summary>
        /// <returns>An XElement </returns>
        public XElement GetXmlTree()
        {
            var namer = new RandomAccountName();
            // if the timezone is set to null then set it GMT
            TimeZone = TimeZone ?? "GMT Standard Time";
            var element = new XElement(Namespaces.NsWindowsAzure + "ConfigurationSet",
                                       new XElement(Namespaces.NsWindowsAzure + "ConfigurationSetType", ConfigurationSetType.ToString()),
                                       new XElement(Namespaces.NsWindowsAzure + "ComputerName", namer.GetPureRandomValue().ToUpper()),
                                       new XElement(Namespaces.NsWindowsAzure + "AdminPassword", AdminPassword),
                                       /*new XElement(Namespaces.NsWindowsAzure + "ResetPasswordOnFirstLogon", ResetPasswordOnFirstLogon.ToString(CultureInfo.InvariantCulture).ToLower()),*/
                                       new XElement(Namespaces.NsWindowsAzure + "EnableAutomaticUpdates", EnableAutomaticUpdate.ToString(CultureInfo.InvariantCulture).ToLower()),
                                       new XElement(Namespaces.NsWindowsAzure + "TimeZone", TimeZone));
            //if (ComputerName != null)
            //    element.Add(new XElement(Namespaces.NsWindowsAzure + "ComputerName",
            //                             ComputerName.ToString(CultureInfo.InvariantCulture)));
            return element;
        }

        #endregion

        #region Overrides of ConfigurationSet

        /// <summary>
        /// Used to set the type of configuration set used with this vm instance
        /// </summary>
        public override ConfigurationSetType ConfigurationSetType
        {
            get { return ConfigurationSetType.WindowsProvisioningConfiguration; }
        }

        #endregion

        // TODO: Add both the Domain Join (optional) and the stored certificates (mandatory) for this 
    }
}